using Marten;
using Pizzeria.Domain;

namespace Pizzeria.Store.Orders;

public class OrderDeliveredEventHandler(IDocumentStore store, ILogger<OrderDeliveredEventHandler> logger)
{
    public async Task Handle(OrderDeliveredEvent @event, CancellationToken cancellationToken)
    {
        using (logger.BeginOrderEventScope(@event))
        using (var session = store.LightweightSession())
        {
            var order = await session.LoadAsync<Order>(@event.OrderId, cancellationToken);

            // There appears to be a bug in Marten where the session.LoadAsync changers the order id to a new guid
            if (order == null || order.Id != @event.OrderId)
            {
                logger.LogError("Order not found");
                return;
            }

            order.PizzasDelivered();

            session.Update(order);

            await session.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order completed");
        }
    }
}
