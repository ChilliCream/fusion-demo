using HotChocolate.Fusion.SourceSchema.Types;

namespace Demo.Accounts.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    [Lookup]
    public static async Task<User?> GetUserById(
        int id,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(id, cancellationToken);

    [Lookup]
    public static async Task<User?> GetUserByUsername(
        string username,
        UserByNameDataLoader userByName,
        CancellationToken cancellationToken)
        => await userByName.LoadAsync(username, cancellationToken);

    [UsePaging]
    public static IQueryable<User> GetUsers(AccountContext context)
        => context.Users.OrderBy(t => t.Name);
}
