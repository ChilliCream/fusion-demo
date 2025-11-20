namespace Demo.Cart.Types;

[QueryType]
public static partial class Query
{
    /// <summary>
    /// Gets the current viewer.
    /// </summary>
    public static Viewer GetViewer()
        => new Viewer();
    
    [Lookup, Internal]
    public static Product? GetProductById(
        [ID<Product>] int id)
        => new(id);
}
