using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Marten;
using Pizzeria.Common;
using Pizzeria.Domain;
using Pizzeria.Middleware;
using Pizzeria.Store;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

var postgreSqlConnectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrWhiteSpace(postgreSqlConnectionString))
{
    throw new ApplicationException("Postgres connection string is missing");
}

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
if (string.IsNullOrWhiteSpace(rabbitMqConnectionString))
{
    throw new ApplicationException("RabbitMQ connection string is missing");
}

builder.Services.AddLogging();

builder.Services.AddMarten(options =>
{
    options.Connection(postgreSqlConnectionString);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.DatabaseSchemaName = "pizzeria";
});

builder.Services.AddCorrelate(options =>
{
    options.RequestHeaders = Correlation.RequestHeaders;
    options.LoggingScopeKey = LoggingScopes.CorrelationId;
});

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(rabbit => rabbit.HostName = rabbitMqConnectionString);
    options.UseFluentValidation();

    options.Policies.AddOrderDomainEventLoggingMiddleware();

    options.PublishMessage<OrderPlacedEvent>()
        .ToRabbitQueue(QueueNames.Orders)
        .UseDurableOutbox();

    options.ListenToRabbitQueue(QueueNames.PreparedOrders)
        .PreFetchCount(10)
        .ListenerCount(5)
        .UseDurableInbox();

    options.PublishMessage<OrderReadyForDeliveryEvent>()
        .ToRabbitQueue(QueueNames.DeliveryOrders)
        .UseDurableOutbox();

    options.ListenToRabbitQueue(QueueNames.CompletedOrders)
        .PreFetchCount(10)
        .ListenerCount(5)
        .UseDurableInbox();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCorrelate();

app.MapWolverineEndpoints();

app.Run();