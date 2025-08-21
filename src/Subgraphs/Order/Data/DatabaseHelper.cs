namespace Demo.Order.Data;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        if (await context.Database.EnsureCreatedAsync())
        {
            await context.Order.AddRangeAsync(
                new Order
                {
                    UserId = 1,
                    Weight = 100,
                    Items = new[]
                    {
                        new OrderItem { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem { Price = 54, Quantity = 1, ProductId = 3 }
                    }
                },
                new Order
                {
                    UserId = 2,
                    Weight = 1000,
                    Items = new[]
                    {
                        new OrderItem() { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem() { Price = 1299.50, Quantity = 1, ProductId = 2 }
                    }
                },
                new Order
                {
                    UserId = 3,
                    Weight = 50,
                    Items = new[]
                    {
                        new OrderItem { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem { Price = 54, Quantity = 1, ProductId = 3 }
                    }
                });
            await context.SaveChangesAsync();
        }
    }
}
