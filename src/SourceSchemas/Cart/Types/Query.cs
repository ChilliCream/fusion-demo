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

    [Lookup, Internal]
    public static async Task<Data.Cart?> GetCartByIdAsync(
        [ID<Data.Cart>] int id,
        CartByIdDataLoader cartById,
        CancellationToken cancellationToken)
        => await cartById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static async Task<Data.CartItem?> GetCartItemByIdAsync(
        [ID<Data.CartItem>] int id,
        CartItemByIdDataLoader cartItemById,
        CancellationToken cancellationToken)
        => await cartItemById.LoadAsync(id, cancellationToken);
}
