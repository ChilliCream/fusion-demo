namespace Demo.Order.Types;

public record User([property: ID] int Id)
{
    [UsePaging]
    public static IQueryable<Data.Order> GetOrders(
        [Parent] User user,
        [Service] OrderContext dbContext) =>
        dbContext.Order
            .Where(t => t.UserId == user.Id)
            .Include(t => t.Items);
}
