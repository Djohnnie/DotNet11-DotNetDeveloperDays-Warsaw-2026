namespace EF11.ComplexTypes.Entities;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Complex types are value objects: they have no key of their own and are stored
    // inline with the owning entity (or inside a JSON column).
    public Address ShippingAddress { get; set; } = null!;

    public Address BillingAddress { get; set; } = null!;
}

public class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public Country Country { get; set; } = null!;
}

public class Country
{
    public string Name { get; set; } = string.Empty;

    public string IsoCode { get; set; } = string.Empty;
}
