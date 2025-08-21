namespace Demo.Accounts.Types;

internal static class UserDataLoaders
{
    [DataLoader]
    public static async Task<Dictionary<string, User>> GetUserByNameAsync(
        IReadOnlyList<string> names,
        AccountContext context,
        CancellationToken cancellationToken)
        => await context.Users
            .Where(t => names.Contains(t.Username))
            .ToDictionaryAsync(t => t.Username!, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, User>> GetUserByIdAsync(
        IReadOnlyList<int> ids,
        AccountContext context,
        CancellationToken cancellationToken)
        => await context.Users
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
