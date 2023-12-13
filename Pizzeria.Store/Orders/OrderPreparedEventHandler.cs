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
        var order = await this.GetPreparedOrder(@event.OrderId, cancellationToken);
        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", @event.OrderId);
            return;
        }

        await messageBus.PublishAsync(new OrderReadyForDeliveryEvent(
            order.Id,
            order.DeliveryAddress,
            @event.CorrelationId));
    }

    private async Task<Order?> GetPreparedOrder(Guid orderId, CancellationToken cancellationToken)
    {
        using (var session = store.LightweightSession())
        {
            var order = await session.LoadAsync<Order>(orderId, cancellationToken);

            if (order == null)
            {
                return null;
            }

            order.PizzasPrepared();

            await session.SaveChangesAsync(cancellationToken);

            return order;
        }
    }
}
