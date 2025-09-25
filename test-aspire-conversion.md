# Testing the Aspire Conversion

This document outlines how to test the Aspire conversion of the Pizzeria application.

## Prerequisites

1. Install the .NET Aspire workload:
   ```bash
   dotnet workload install aspire
   ```

2. Trust the .NET development certificates:
   ```bash
   dotnet dev-certs https --trust
   ```

## Verification Steps

### 1. Build the Solution
```bash
dotnet build
```
**Expected**: All projects should build successfully without errors.

### 2. Start the Aspire Application
```bash
dotnet run --project Pizzeria.AppHost
```
**Expected**: 
- Aspire Dashboard should start (typically at https://localhost:15888)
- All infrastructure services (PostgreSQL, RabbitMQ, Seq) should be provisioned
- All application services should start and show as healthy

### 3. Test the Pizza Ordering Flow

1. Navigate to the Aspire Dashboard
2. Find the `pizzeria-store` service and click on its endpoint
3. Navigate to the Swagger UI (`/swagger`)
4. Test the `GET /menu` endpoint to retrieve the pizza menu
5. Test the `POST /orders` endpoint with this payload:
   ```json
   {
     "customerName": "Kobbie Mainoo",
     "deliveryAddress": "Locker 37, Home Dressing Room, Old Trafford",
     "pizzaIds": [3, 7]
   }
   ```

### 4. Monitor the Message Flow

In the Aspire Dashboard, check the logs for each service:

1. **Pizzeria.Store**: Should log order placement and database save
2. **Pizzeria.Kitchen**: Should receive order message and simulate pizza preparation (5 seconds per pizza)
3. **Pizzeria.Store**: Should receive prepared order message and update order status
4. **Pizzeria.Delivery**: Should receive delivery message and simulate delivery
5. **Pizzeria.Store**: Should receive completion message and mark order as delivered

### 5. Expected Log Sequence

Look for these log messages in order:

1. Store: "Order {OrderId} placed"
2. Kitchen: "Preparing pizza {PizzaId}" (for each pizza)
3. Kitchen: "Pizza {PizzaId} is ready" (for each pizza)
4. Kitchen: "Order has been prepared"
5. Store: "Order completed"

## Infrastructure Service Access

- **Aspire Dashboard**: https://localhost:15888
- **PostgreSQL**: Available through Aspire dashboard connection strings
- **RabbitMQ Management**: Available through Aspire dashboard
- **Seq Logging**: Available through Aspire dashboard

## Troubleshooting

### Common Issues

1. **Port conflicts**: Aspire uses dynamic port allocation, but if you encounter conflicts, restart the AppHost
2. **Container startup delays**: Infrastructure services may take a few moments to be ready
3. **Certificate issues**: Ensure dev certificates are trusted with `dotnet dev-certs https --trust`

### Fallback to Docker Compose

If Aspire doesn't work in your environment, you can still use the traditional Docker Compose setup:

```bash
docker-compose up -d
dotnet run --project Pizzeria.Store &
dotnet run --project Pizzeria.Kitchen &
dotnet run --project Pizzeria.Delivery &
```

Then test the same endpoints at the individual service URLs.