using System.Text.Json.Serialization;

namespace Pizzeria.Domain;

public class Order
{
    public Order(string customerName, string deliveryAddress, Pizza[] pizzas)
    {
        this.Id = Guid.NewGuid();
        this.OrderDate = DateTime.UtcNow;

        this.CustomerName = customerName;
        this.DeliveryAddress = deliveryAddress;
        this.Pizzas = pizzas;
    }

    [JsonInclude]
    public Guid Id { get; protected set; }

    [JsonInclude]
    public DateTime OrderDate { get; protected set; }

    [JsonInclude]
    public string CustomerName { get; protected set; }

    [JsonInclude]
    public string DeliveryAddress { get; protected set; }

    [JsonInclude]
    public Pizza[] Pizzas { get; protected set; }

    public decimal TotalPrice => this.Pizzas.Sum(x => x.Price);

    [JsonInclude]
    public bool IsPrepared { get; protected set; }

    [JsonInclude]
    public DateTime? PreparationDate { get; protected set; }

    [JsonInclude]
    public bool IsDelivered { get; protected set; }

    [JsonInclude]
    public DateTime? DeliveryDate { get; protected set; }

    public void PizzasPrepared()
    {
        if (this.IsPrepared)
        {
            return;
        }

        this.IsPrepared = true;
        this.PreparationDate = DateTime.UtcNow;
    }

    public void PizzasDelivered()
    {
        if (!this.IsPrepared)
        {
            throw new InvalidOperationException("Cannot deliver an order that is not prepared.");
        }

        if (this.IsDelivered)
        {
            return;
        }

        this.IsDelivered = true;
        this.DeliveryDate = DateTime.UtcNow;
    }
}
