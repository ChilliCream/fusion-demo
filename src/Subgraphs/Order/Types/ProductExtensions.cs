namespace Demo.Order.Types;

[ExtendObjectType<Product>]
public static class ProductExtensions
{
    [UsePaging]
    public static IQueryable<Data.Order> GetOrders(
        [Parent] Product product,
        [Service] OrderContext dbContext) =>
        dbContext.Order
            .Where(t => t.Items.Any(t => t.ProductId == product.Id))
            .Include(t => t.Items);
}
