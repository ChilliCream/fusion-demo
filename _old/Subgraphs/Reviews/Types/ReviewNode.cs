namespace Demo.Reviews.Types;

[ExtendObjectType<Review>(IgnoreProperties = new[] { nameof(Review.AuthorId), nameof(Review.ProductId) })]
internal static class ReviewNode
{
    [Tag("internal")]
    public static string ThisIsInternal() => "This is internal";

    public static Product GetProduct(
        [Parent] Review review)
        => new(review.ProductId);

    public static async Task<User> GetAuthorAsync(
        [Parent] Review review,
        UserByIdDataLoader userDataLoader,
        CancellationToken cancellationToken)
        => await userDataLoader.LoadAsync(review.AuthorId, cancellationToken);

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, Review>> GetReviewByIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Reviews
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

    [DataLoader]
    internal static async Task<ILookup<int, Review>> GetReviewsByUserIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
    {
        var reviews = await context.Users
            .Where(t => ids.Contains(t.Id))
            .SelectMany(t => t.Reviews)
            .ToListAsync(cancellationToken);

        return reviews.ToLookup(t => t.AuthorId);
    }

    public static string FooBarBaz => "FooBarBaz";
}
