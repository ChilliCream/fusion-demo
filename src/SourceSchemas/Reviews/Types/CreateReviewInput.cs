namespace Demo.Reviews.Types;

public record CreateReviewInput(
    string Body, 
    int Stars, 
    int ProductId, 
    [property: ID<Author>] int AuthorId);