using GreenDonut.Data;
using HotChocolate.Types.Pagination;

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
    public static async Task<Connection<User>> GetUsers(
        PagingArguments pagingArgs,
        AccountContext context,
        CancellationToken cancellationToken) 
        => await context.Users
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(pagingArgs, cancellationToken)
            .ToConnectionAsync();
}
