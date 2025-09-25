# Critter Stack Sample/Experiment

This repo contains a sample application that I used to experiment with the [Marten](https://martendb.io/) and [Wolverine](https://wolverine.netlify.app/) running on .NET Aspire.

## Application Architecture

The application is a simple Pizzeria that takes orders via a REST API.

`Marten` is used to store the orders and `Wolverine` publish and subscribe to messages on RabbitMQ.

There are the following microservices in the application.

- `Pizzeria.Store`
  - Has to REST endpoints:
    - `GET /menu` - Returns the menu
	- `POST /orders` - Takes an order
- `Pizzeria.Kitchen`
  - Simulates the cooking of pizzas
- `Pizzeria.Delivery`
  - Simulates the delivery of orders

## Sequence of Events

- `Pizzeria.Store` receives an order on the `POST /orders` endpoint, stores as a document in the database, and puts a message on the `orders` queue.
- `Pizzeria.Kitchen` receives the messages off the `orders` queue, simulates the preparation of the pizzas, and then puts a messages on the `prepared-orders` queue.
- `Pizzeria.Store` receives the message off the `prepared-orders` queue, updates the order document to marks it as prepared, and puts a message on the `delivery-orders` queue.
- `Pizzeria.Delivery` receives the messages off the `delivery-orders` queue, simulates the delivery of the order, and then puts a messages on the `completed-orders` queue.
- `Pizzeria.Store` finally received messages off the `completed-orders` queue and updates the order document to mark them as delivered.

## Running the Application

### Prerequisites

1. Install the .NET Aspire workload:
   ```bash
   dotnet workload install aspire
   ```

2. Trust the .NET development certificates:
   ```bash
   dotnet dev-certs https --trust
   ```

### Starting the Application

1. Clone this repo.
2. Start the Aspire application:
   ```bash
   dotnet run --project ./Pizzeria.AppHost/Pizzeria.AppHost.csproj
   ```
3. Navigate to the Aspire dashboard (typically at `https://localhost:15888`) to monitor all services.
4. Open the `Pizzeria.Store` service from the dashboard and navigate to its Swagger UI.
5. Send an order to the Pizza Store using the `POST /orders` endpoint with the following JSON:
   ```json
   {
     "customerName": "Kobbie Mainoo",
     "deliveryAddress": "Locker 37, Home Dressing Room, Old Trafford",
     "pizzaIds": [3, 7]
   }
   ```
6. Check the logs in the Aspire dashboard for each service to see the sequence of events.

### Infrastructure Services

The application uses the following infrastructure services managed by Aspire:

- **PostgreSQL** - Database for storing orders
- **RabbitMQ** - Message broker for service communication  
- **Seq** - Centralized logging

All services are automatically configured with service discovery and connection strings are managed by Aspire.

## Traditional Docker Compose Setup

If you prefer to run the application without Aspire, you can still use the traditional approach:

1. Run `docker-compose up` to start the RabbitMQ, PostgreSQL and Seq containers.
2. Run `dotnet run --project ./Pizzeria.Store/Pizzeria.Store.csproj` to start the `Pizzeria.Store` service.
3. Run `dotnet run --project ./Pizzeria.Kitchen/Pizzeria.Kitchen.csproj` to start the `Pizzeria.Kitchen` service.
4. Run `dotnet run --project ./Pizzeria.Delivery/Pizzeria.Delivery.csproj` to start the `Pizzeria.Delivery` service.
5. Check the logs in [Seq](http://localhost:5341/#/events) for each service to see the sequence of events.
