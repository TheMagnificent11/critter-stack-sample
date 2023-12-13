using Pizzeria.Common;

namespace Pizzeria.Domain;

public interface IOrderDomainEvent : IDomainEvent
{
    Guid OrderId { get; }
}
