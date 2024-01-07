using Marten;
using Pizzeria.Domain;
using Wolverine;

namespace Pizzeria.Store.Orders;

public class OrderPreparedEventHandler(
    IDocumentStore store,
    IMessageBus messageBus,
    ILogger<OrderPreparedEventHandler> logger)
{
    public async Task Handle(OrderPreparedEvent @event, CancellationToken cancellationToken)
    {
        using (logger.BeginOrderEventScope(@event))
        {
            var order = await this.GetPreparedOrder(@event.OrderId, cancellationToken);
            if (order == null)
            {
                logger.LogError("Order not found");
                return;
            }

            await messageBus.PublishAsync(new OrderReadyForDeliveryEvent(
                @event.OrderId,
                order.DeliveryAddress,
                @event.CorrelationId));
        }
    }

    private async Task<Order?> GetPreparedOrder(Guid orderId, CancellationToken cancellationToken)
    {
        using (var session = store.LightweightSession())
        {
            var order = await session.LoadAsync<Order>(orderId, cancellationToken);

            if (order == null || order.Id != orderId)
            {
                return null;
            }

            order.PizzasPrepared();

            session.Update(order);

            await session.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order is ready for delivery");

            return order;
        }
    }
}
