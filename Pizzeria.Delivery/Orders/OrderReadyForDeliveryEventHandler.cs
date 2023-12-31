﻿using Pizzeria.Domain;
using Wolverine;

namespace Pizzeria.Delivery.Orders;

public class OrderReadyForDeliveryEventHandler(
    IMessageBus messageBus,
    ILogger<OrderReadyForDeliveryEventHandler> logger)
{
    public async Task Handle(OrderReadyForDeliveryEvent @event, CancellationToken cancellationToken)
    {
        using (logger.BeginOrderEventScope(@event))
        {
            // Wait a random amount of time between 5 and 10 seconds
            var random = new Random();
            var seconds = random.Next(5, 10);
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

            logger.LogInformation("Order has been delivered");

            await messageBus.PublishAsync(new OrderDeliveredEvent(@event.OrderId, @event.CorrelationId));
        }
    }
}
