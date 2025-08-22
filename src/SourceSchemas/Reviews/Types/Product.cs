using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Reviews.Types;

public sealed record Product([property: ID<Product>] int Id)
{
    [UsePaging(ConnectionName = "ProductReviews")]
    public async Task<Connection<Review>> GetReviewsAsync(
        PagingArguments pagingArgs,
        QueryContext<Review> context,
        IReviewsByProductIdDataLoader reviewByProductId,
        CancellationToken cancellationToken = default)
        => await reviewByProductId
            .With(pagingArgs, context)
            .LoadAsync(Id, cancellationToken)
            .ToConnectionAsync();
}