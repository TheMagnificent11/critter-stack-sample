# Critter Stack Sample/Experiment

This repo contains a sample application that I used to experiment with the [Marten](https://martendb.io/) and [Wolverine](https://wolverine.netlify.app/).

## Application Architecture

The application is a simple Pizzeria that takes orders via a REST API.

`Marten` is used to store the orders and `Wolverine` publish and subscribe to messages on RabbitMQ.

There are the following microservices in the application.

- `Pizzeria.Store`
  - Has to REST endpoints:
    - `GET /menu` - Returns the menu
	- `POST /order` - Takes an order
- `Pizzeria.Kitchen`
  - Simulates the cooking of pizzas
- `Pizzeria.Delivery`
  - Simulates the delivery of orders

## Sequence of Events

- `Pizzeria.Store` receives an order on the `POST /orders` endpoint, stores as a document in the database, and puts a message on the `orders` queue.
- `Pizzeria.Kitchen` receives the messages off the `orders` queue, simulates the preparation of the pizzas, and then puts a messages on the `prepared-orders` queue.
- `Pizzeria.Store` receives the message off the `prepared-orders` queue, updates the order document to marks it as prepared, and puts a message on the `delivery-orders` queue.
- `Pizzeria.Delivery` receives the messages off the `delivery-orders` queue, simulates the delivery of the order, and then puts a messages on the `completed-orders` queue.
- `Pizzeria.Store` finally received messages off the `completed-orders` queue and updates the order document to mark them as delivered (currently contains a [bug](#bug)).

## Running the Application

1. Clone this repo.
2. Run `docker-compose up` to start the RabbitMQ, PostgreSQL and Seq containers.
3. Run `dotnet run --project ./Pizzeria.Store/Pizzeria.Store.csproj` to start the `Pizzeria.Store` service.
4. Run `dotnet run --project ./Pizzeria.Kitchen/Pizzeria.Kitchen.csproj` to start the `Pizzeria.Kitchen` service.
5. Run `dotnet run --project ./Pizzeria.Delivery/Pizzeria.Delivery.csproj` to start the `Pizzeria.Delivery` service.
6. Send an order to the Pizza Store using the `POST /order` endpoint
7. Check the logs in [Seq](http://localhost:5341/#/events) for each service to see the sequence of events.

## Bug

The `Order` class is implemented using domain-driven design principles where not every property can be set during initialization.

`Marten` can't initialize properties that cannot be set by JSON deserialization.

Therefore, `Order.IsPrepared` is true in the database, but false on the `Order` instance read from the database when handling messages off the `completed-orders` queue.
