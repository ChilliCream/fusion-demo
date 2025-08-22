namespace Demo.Order.Types;

public record OrderItemInput(
    [property: ID<Product>] int ProductId,
    int Quantity,
    double Price);