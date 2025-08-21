using HotChocolate.Fusion.SourceSchema.Types;

namespace Shipping.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    [Lookup]
    public static Product GetProductById(int id)
        => new(id);

    public static string ShippingVersion() => "1.0.0";
}

public sealed record Product([property: ID<Product>] int Id)
{
    public DeliveryEstimate GetDeliveryEstimate(string zip)
    {
        return new DeliveryEstimate(123, 123);
    }
}

public sealed record DeliveryEstimate(int Min, int Max);
