var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("seq", "datalust/seq:latest")
    .WithServiceBinding(containerPort: 80, hostPort: 5341, name: "http", scheme: "http");

//var rabbitMq = builder.AddRabbitMQ("rabbitmq");
builder.AddContainer("rabbitmq", "rabbitmq:3.8.9-management-alpine")
    .WithServiceBinding(containerPort: 5672, hostPort: 5672, name: "http1", scheme: "http")
    .WithServiceBinding(containerPort: 15672, hostPort: 15672, name: "http2", scheme: "http")
    .WithVolumeMount("./rabbitmq.conf", "/etc/rabbitmq/rabbitmq.conf", VolumeMountType.Bind)
    .WithVolumeMount("./rabbitmq-definitions.json", "/etc/rabbitmq/definitions.json", VolumeMountType.Bind);

//var postgres = builder
//    .AddPostgres("pg")
//    .AddDatabase("postgresdb");
builder.AddContainer("postgres", "postgres:13.1-alpine")
    .WithServiceBinding(containerPort: 5432, hostPort: 5432, name: "http", scheme: "http")
    .WithVolumeMount("./postgres.conf", "/etc/postgresql/postgresql.conf", VolumeMountType.Bind)
    .WithVolumeMount("./postgres-init.sql", "/docker-entrypoint-initdb.d/postgres-init.sql", VolumeMountType.Bind);

builder
    .AddProject<Projects.Pizzeria_Store>("store");
    //.WithReference(postgres)
    //.WithReference(rabbitMq);

builder
    .AddProject<Projects.Pizzeria_Kitchen>("kitchen");
//.WithReference(rabbitMq);

builder
    .AddProject<Projects.Pizzeria_Delivery>("delivery");
    //.WithReference(rabbitMq);

builder.Build().Run();
