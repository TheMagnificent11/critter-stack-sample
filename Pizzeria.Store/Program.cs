using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using JasperFx;
using Marten;
using Marten.Services.Json;
using Pizzeria.Common;
using Pizzeria.Domain;
using Pizzeria.Store;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var postgresSqlConnectionString = builder.Configuration.GetConnectionString(ServiceNames.DatabaseServer);
if (string.IsNullOrWhiteSpace(postgresSqlConnectionString))
{
    throw new ApplicationException("Postgres connection string is missing");
}

var rabbitMqConnectionString = builder.Configuration.GetConnectionString(ServiceNames.MessageBroker);
if (string.IsNullOrWhiteSpace(rabbitMqConnectionString))
{
    throw new ApplicationException("RabbitMQ connection string is missing");
}

builder.Services.AddLogging(options => options.AddSeq());

builder.Services.AddMarten(options =>
{
    options.Connection(postgresSqlConnectionString);
    options.AutoCreateSchemaObjects = JasperFx.AutoCreate.All;
    options.UseSystemTextJsonForSerialization();
});

builder.Services.AddCorrelate(options =>
{
    options.RequestHeaders = Correlation.RequestHeaders;
    options.LoggingScopeKey = LoggingScopes.CorrelationId;
});

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(configuration =>
    {
        configuration.HostName = rabbitMqConnectionString;
    });
    options.UseFluentValidation();

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
builder.Services.AddWolverineHttp();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCorrelate();

app.MapWolverineEndpoints();

app.Run();
