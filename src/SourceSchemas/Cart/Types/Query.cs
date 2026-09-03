using Demo.Cart.Data;

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

    // The gateway calls this only to complete entities carried by another
    // subgraph's reference (e.g. Promotions extending Cart); it never runs
    // ownership checks, since it is @internal and not reachable by clients.
    [Lookup, Internal]
    public static async Task<Data.Cart?> GetCartByIdAsync(
        [ID<Data.Cart>] int id,
        CartByIdDataLoader cartById,
        CancellationToken cancellationToken)
        => await cartById.LoadAsync(id, cancellationToken);
}
