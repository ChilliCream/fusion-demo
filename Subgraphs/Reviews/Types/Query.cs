namespace Demo.Reviews.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    public static async Task<Review?> GetReviewById(
        int id,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(id, cancellationToken);

    [UsePaging]
    public static IQueryable<Review> GetReviews(ReviewContext context)
        => context.Reviews.OrderByDescending(t => t.Id);

    [NodeResolver]
    public static async Task<User?> GetUserById(
        int id,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(id, cancellationToken);

    public static async Task<IReadOnlyList<User>> GetUsersById(
        [ID<User>] int[] ids,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(ids, cancellationToken);

    [UsePaging]
    public static IQueryable<User> GetUsers(ReviewContext context)
        => context.Users.OrderBy(t => t.Name);

    public static Product GetProductById([ID<Product>] int id)
        => new Product(id);

    public static string More => "more";
}