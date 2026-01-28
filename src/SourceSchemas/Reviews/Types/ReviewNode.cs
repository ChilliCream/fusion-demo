namespace Demo.Reviews.Types;

[ObjectType<Review>]
internal static partial class ReviewNode
{
    [BindMember(nameof(Review.AuthorId))]
    public static async Task<User?> GetAuthorAsync(
        [Parent(requires: nameof(Review.AuthorId))] Review review,
        IUserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(review.AuthorId, cancellationToken);
}
