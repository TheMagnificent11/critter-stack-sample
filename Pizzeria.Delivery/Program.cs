using Pizzeria.Common;
using Pizzeria.Domain;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options => options.AddSeq());

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(rabbit => rabbit.HostName = builder.Configuration.GetConnectionString("RabbitMQ"));

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
