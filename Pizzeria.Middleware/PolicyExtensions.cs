using Pizzeria.Domain;
using Wolverine;

namespace Pizzeria.Middleware;

public static class PolicyExtensions
{
    public static void AddOrderDomainEventLoggingMiddleware(this IPolicies policies)
    {
        policies
            .ForMessagesOfType<IOrderDomainEvent>()
            .AddMiddleware(typeof(OrderDomainEventLoggingMiddleware));
    }
}
