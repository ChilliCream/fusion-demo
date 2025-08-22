namespace Demo.Reviews.Types;

[QueryType]
public static partial class UserQueries
{
    [Lookup, Internal]
    public static async Task<User?> GetUserById(
        [ID<Product>] int id,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(id, cancellationToken);
}
