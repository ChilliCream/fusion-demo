namespace Demo.Reviews.Types;

[QueryType]
public static partial class ProductQueries
{
    [Lookup, Internal]
    public static Product GetProductById([ID<Product>] int id)
        => new(id);
}
