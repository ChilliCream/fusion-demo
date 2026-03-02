namespace Demo.Reviews.Data;

internal static class AuthorDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Author>> GetUserByIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Users
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}