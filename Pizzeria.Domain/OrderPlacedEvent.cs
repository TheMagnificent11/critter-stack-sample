﻿namespace Pizzeria.Domain;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
public record OrderPlacedEvent(Guid OrderId, Pizza[] Pizzas, string CorrelationId) : IOrderDomainEvent;
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
