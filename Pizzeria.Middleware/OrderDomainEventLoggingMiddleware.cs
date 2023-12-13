using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Domain;
using Wolverine;

namespace Pizzeria.Middleware;

public static class OrderDomainEventLoggingMiddleware
{
    public static (HandlerContinuation, IDomainEvent) Handle(IOrderDomainEvent domainEvent, ILogger logger)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            { LoggingScopes.CorrelationId, domainEvent.CorrelationId },
            { LoggingScopes.OrderId, domainEvent.OrderId }
        }))
        {
            return (HandlerContinuation.Continue, domainEvent);
        }
    }
}
