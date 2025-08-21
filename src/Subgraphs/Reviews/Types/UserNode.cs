namespace Demo.Reviews.Types;

[ExtendObjectType<User>]
internal static class UserNode
{
    [UsePaging]
    public static async Task<IEnumerable<Review?>?> GetReviewsAsync(
        [Parent] User user,
        ReviewsByUserIdDataLoader reviewsById,
        CancellationToken cancellationToken)
        => await reviewsById.LoadAsync(user.Id, cancellationToken);

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, User>> GetUserByIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Users
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
