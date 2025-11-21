namespace Demo.Cart.Types;

public sealed class ProductAmountCannotBeLowerThanOneException(int productId, int amount) 
    : Exception($"The amount of product with ID '{productId}' cannot be lower than one. Given amount: {amount}.");
