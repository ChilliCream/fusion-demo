namespace Demo.Order.Types;

public record OrderItemInput(
    int ProductId,
    int Quantity,
    double Price);