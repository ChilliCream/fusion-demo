namespace Demo.Reviews.Data;

internal static class UserDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, User>> GetUserByIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Users
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}