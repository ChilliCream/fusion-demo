namespace Demo.Order.Types;

[QueryType]
public static partial class OrderQueries
{
    [NodeResolver]
    public static async Task<Data.Order?> GetOrderByIdAsync(
        int id,
        IOrderByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);
}
