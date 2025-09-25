using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add infrastructure services
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("pizzeria");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var seq = builder.AddSeq("seq");

// Add application services
var store = builder.AddProject<Projects.Pizzeria_Store>("pizzeria-store")
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(seq);

builder.AddProject<Projects.Pizzeria_Kitchen>("pizzeria-kitchen")
    .WithReference(rabbitmq)
    .WithReference(seq);

builder.AddProject<Projects.Pizzeria_Delivery>("pizzeria-delivery")
    .WithReference(rabbitmq)
    .WithReference(seq);

builder.Build().Run();
