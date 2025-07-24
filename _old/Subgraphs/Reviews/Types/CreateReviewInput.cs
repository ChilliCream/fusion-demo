namespace Demo.Reviews.Types;

public record CreateReviewInput(
    string Body, 
    int Stars, 
    [property: ID<Product>] int ProductId, 
    [property: ID<User>] int AuthorId);