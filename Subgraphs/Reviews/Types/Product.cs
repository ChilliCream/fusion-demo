namespace Demo.Reviews.Types;

public sealed class Product
{
    public Product(int id)
    {
        Id = id;
    }

    [ID<Product>] public int Id { get; }

    [UsePaging(ConnectionName = "ProductReviews")]
    public IQueryable<Review> GetReviews(ReviewContext context)
        => context.Reviews;
}
