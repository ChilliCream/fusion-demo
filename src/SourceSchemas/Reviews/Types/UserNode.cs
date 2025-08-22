using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Reviews.Types;

[ObjectType<User>]
internal static partial class UserNode
{
    static partial void Configure(IObjectTypeDescriptor<User> descriptor)
        => descriptor.Field(x => x.Id).ID();

    [UsePaging(ConnectionName = "UserReviews")]
    public static async Task<Connection<Review>> GetReviewsAsync(
        [Parent(requires: nameof(User.Id))] User user,
        PagingArguments arguments,
        QueryContext<Review> query,
        ReviewsByUserIdDataLoader reviewsByUserId,
        CancellationToken cancellationToken)
        => await reviewsByUserId
            .With(arguments, query)
            .LoadAsync(user.Id, cancellationToken)
            .ToConnectionAsync();
}