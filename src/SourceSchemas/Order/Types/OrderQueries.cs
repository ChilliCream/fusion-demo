namespace Demo.Order.Types;

[QueryType]
public static partial class OrderQueries
{
    [Lookup, NodeResolver]
    public static async Task<Data.Order?> GetOrderByIdAsync(
        int id,
        IOrderByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static User GetUserById([ID<User>] int id) 
        => new(id);
}
