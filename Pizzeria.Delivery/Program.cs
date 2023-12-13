using Pizzeria.Common;
using Pizzeria.Domain;
using Pizzeria.Middleware;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(rabbit => rabbit.HostName = builder.Configuration.GetConnectionString("RabbitMQ"));

    options.Policies.AddOrderDomainEventLoggingMiddleware();

    options.ListenToRabbitQueue(QueueNames.DeliveryOrders)
        .PreFetchCount(10)
        .ListenerCount(5)
        .UseDurableInbox();

    options.PublishMessage<OrderDeliveredEvent>()
        .ToRabbitQueue(QueueNames.CompletedOrders)
        .UseDurableOutbox();
});

var app = builder.Build();

app.Run();
