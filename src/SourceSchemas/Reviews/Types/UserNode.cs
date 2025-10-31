using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Reviews.Types;

[ObjectType<User>]
internal static partial class UserNode
{
    [ID]
    public static int GetId([Parent] User user)
        => user.Id;
    
    [Shareable]
    public static string GetName([Parent] User user)
        => user.Name!;

    [UsePaging(ConnectionName = "UserReviews")]
    public static async Task<Connection<Review>> GetReviewsAsync(
        [Parent(requires: nameof(User.Id))] User user,
        PagingArguments arguments,
        QueryContext<Review> query,
        IReviewsByUserIdDataLoader reviewsByUserId,
        CancellationToken cancellationToken)
        => await reviewsByUserId
            .With(arguments, query)
            .LoadAsync(user.Id, cancellationToken)
            .ToConnectionAsync();
}