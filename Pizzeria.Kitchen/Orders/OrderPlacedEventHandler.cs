using Pizzeria.Domain;
using Wolverine;

namespace Pizzeria.Kitchen.Orders;

public class OrderPlacedEventHandler(IMessageBus messageBus, ILogger<OrderPlacedEventHandler> logger)
{
    public async Task Handle(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        using (logger.BeginOrderEventScope(@event))
        {
            if (@event.Pizzas.Length == 0)
            {
                logger.LogWarning("Order has no pizzas");
                return;
            }

            foreach (var item in @event.Pizzas)
            {
                logger.LogInformation("Preparing pizza {PizzaId}", item.Id);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                logger.LogInformation("Pizza {PizzaId} is ready", item.Id);

                await messageBus.PublishAsync(new OrderPreparedEvent(@event.OrderId, @event.CorrelationId));
            }
        }
    }
}
