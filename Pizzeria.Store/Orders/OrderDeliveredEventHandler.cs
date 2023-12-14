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

            if (order == null)
            {
                logger.LogError("Order {OrderId} not found", @event.OrderId);
                return;
            }

            order.PizzasDelivered();

            await session.SaveChangesAsync(cancellationToken);
        }
    }
}
