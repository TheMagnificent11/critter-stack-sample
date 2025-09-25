using Pizzeria.Common;
using Pizzeria.Domain;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddLogging(options => options.AddSeq());

builder.Host.UseWolverine(options =>
{
    var rabbitMqConnectionString = builder.Configuration.GetConnectionString(ServiceNames.MessageBroker);
    if (string.IsNullOrWhiteSpace(rabbitMqConnectionString))
    {
        throw new ApplicationException("RabbitMQ connection string is missing");
    }

    options.UseRabbitMq(rabbitMqConnectionString)
        .AutoProvision()
        .AutoPurgeOnStartup();

    options.ListenToRabbitQueue(QueueNames.DeliveryOrders)
        .PreFetchCount(10)
        .ListenerCount(5)
        .UseDurableInbox();

    options.PublishMessage<OrderDeliveredEvent>()
        .ToRabbitQueue(QueueNames.CompletedOrders)
        .UseDurableOutbox();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
