namespace Demo.Products.Types;

public sealed record UnknownProductError(int ProductId)
{
    public string Message => $"The product with the id {ProductId} does not exist.";
}