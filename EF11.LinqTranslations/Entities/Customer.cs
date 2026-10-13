namespace EF11.LinqTranslations.Entities;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public List<Order> Orders { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public DateTime PlacedOn { get; set; }

    public decimal Total { get; set; }
}
