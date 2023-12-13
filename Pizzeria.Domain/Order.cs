namespace Pizzeria.Domain;

public class Order(string customerName, string deliveryAddress, Pizza[] pizzas)
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime OrderDate { get; protected set; } = DateTime.UtcNow;
    public string CustomerName { get; protected set; } = customerName;
    public string DeliveryAddress { get; protected set; } = deliveryAddress;
    public Pizza[] Pizzas { get; protected set; } = pizzas;
    public decimal TotalPrice => this.Pizzas.Sum(x => x.Price);
    public bool IsPrepared { get; protected set; }
    public DateTime? PreparationDate { get; protected set; }
    public bool IsDelivered { get; protected set; }
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
