using Correlate;
using Marten;
using Pizzeria.Domain;
using Wolverine;
using Wolverine.Http;

namespace Pizzeria.Store.Orders;

public class PlaceOrderEndpoint(
    IDocumentStore store,
    ICorrelationContextAccessor correlationContextAccessor,
    IMessageBus messageBus,
    ILogger<PlaceOrderEndpoint> logger)
{
    [WolverinePost("/orders")]
    public async Task Post(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        using (var session = store.LightweightSession())
        {
            var pizzas = Domain.Menu.Pizzas
                .Where(x => command.PizzaIds.Contains(x.Id))
                .ToArray();
            var order = new Order(command.CustomerName, command.DeliveryAddress, pizzas);

            session.Store(order);

            await session.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order {OrderId} placed", order.Id);

            var orderPlacedEvent = new OrderPlacedEvent(
                order.Id,
                order.Pizzas,
                correlationContextAccessor?.CorrelationContext?.CorrelationId ?? Guid.NewGuid().ToString());

            await messageBus.PublishAsync(orderPlacedEvent);
        }
    }
}
