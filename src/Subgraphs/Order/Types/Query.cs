namespace Demo.Order.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    public static async Task<Data.Order?> GetOrderByIdAsync(
        int id,
        OrderByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    public static Product GetProductById([ID<Product>] int id)
    {
        return new Product(id);
    }

    public static IEnumerable<Product> GetProductsByIds([ID<Product>] IEnumerable<int> ids)
    {
        return ids.Select(id => new Product(id));
    }

    public static User GetUserById([ID<User>] int id)
    {
        return new User(id);
    }

    public static IEnumerable<User> GetUsersByIds([ID<User>] IEnumerable<int> ids)
    {
        return ids.Select(id => new User(id));
    }
}
