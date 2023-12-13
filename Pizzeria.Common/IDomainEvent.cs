namespace Pizzeria.Common;

public interface IDomainEvent
{
    string CorrelationId { get; }
}
