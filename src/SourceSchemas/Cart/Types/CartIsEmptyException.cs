namespace Demo.Cart.Types;

public sealed class CartIsEmptyException()
    : Exception("The cart has no items to check out.");
