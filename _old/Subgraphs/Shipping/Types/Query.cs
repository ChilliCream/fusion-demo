namespace Shipping.Types;

[QueryType]
public static class Query
{
    public static Product GetProductById([ID<Product>] int id)
        => new(id);
}

public sealed record Product([property: ID<Product>] int Id)
{
    public DeliveryEstimate GetDeliveryEstimate(string zip, int weight, int size)
        => new(1 * (weight + size), 2 * (weight + size));
}

public sealed record DeliveryEstimate(int Min, int Max);
