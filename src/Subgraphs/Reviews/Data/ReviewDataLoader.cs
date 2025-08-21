using GreenDonut.Data;
using static Demo.Reviews.Data.ReviewOrdering;

namespace Demo.Reviews.Data;

internal static class ReviewDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Review>> GetReviewByIdAsync(
        IReadOnlyList<int> ids,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Reviews
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, Page<Review>>> GetReviewsByUserIdAsync(
        IReadOnlyList<int> ids,
        PagingArguments pagingArgs,
        QueryContext<Review> query,
        ReviewContext context,
        CancellationToken cancellationToken)
        => await context.Reviews
            .Where(u => ids.Contains(u.AuthorId))
            .With(query.Include(t => t.ProductId), DefaultOrder)
            .ToBatchPageAsync(t => t.ProductId, pagingArgs, cancellationToken);
    
    [DataLoader]
    public static async Task<Dictionary<int, Page<Review>>> GetReviewsByProductIdAsync(
        IReadOnlyList<int> productIds,
        PagingArguments pagingArgs,
        QueryContext<Review> query,
        ReviewContext context,
        CancellationToken cancellationToken = default)
        => await context.Reviews
            .Where(r => productIds.Contains(r.ProductId))
            .With(query.Include(t => t.ProductId), DefaultOrder)
            .ToBatchPageAsync(t => t.ProductId, pagingArgs, cancellationToken);
}