using Microsoft.Extensions.Logging;
using Pizzeria.Common;

namespace Pizzeria.Domain;

public static class OrderDomainEventLoggingExtensions
{
    public static IDisposable BeginOrderEventScope(this ILogger logger, IOrderDomainEvent @event)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return logger.BeginScope(new Dictionary<string, object>
        {
            { LoggingScopes.CorrelationId, @event.CorrelationId },
            { LoggingScopes.OrderId, @event.OrderId }
        });
#pragma warning restore CS8603 // Possible null reference return.
    }
}
