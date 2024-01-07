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

            if (order == null || order.Id != @event.OrderId)
            {
                logger.LogError("Order not found");
                return;
            }

            // `Marten` can't initialize properties that cannot be set by JSON deserialization.
            // So using DDD, not every property is settable and thus `Pizza.IsPrepared` is true in the database,
            // but false here, leading to an exception been thrown by this method.
            order.PizzasDelivered();

            session.Update(order);

            await session.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order completed");
        }
    }
}
