using Aspire.Hosting;
using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

// Add infrastructure services
var databaseServer = builder.AddPostgres(ServiceNames.DatabaseServer)
    .WithPgAdmin()
    .AddDatabase(ServiceNames.StoreDatabase);

var messageBroker = builder.AddRabbitMQ(ServiceNames.MessageBroker)
    .WithManagementPlugin();

var seq = builder.AddSeq(ServiceNames.Logging);

// Add application services
var store = builder.AddProject<Projects.Pizzeria_Store>(ServiceNames.Store)
    .WithReference(databaseServer)
    .WithReference(messageBroker)
    .WithReference(seq);

builder.AddProject<Projects.Pizzeria_Kitchen>(ServiceNames.Kitchen)
    .WithReference(messageBroker)
    .WithReference(seq);

builder.AddProject<Projects.Pizzeria_Delivery>(ServiceNames.Delivery)
    .WithReference(messageBroker)
    .WithReference(seq);

builder.Build().Run();
