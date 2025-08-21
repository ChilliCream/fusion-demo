namespace Demo.Shipping.Types;

[QueryType]
public static partial class Query
{
    [Lookup, Internal]
    public static Product GetProductById([ID<Product>] int id)
        => new(id);
}