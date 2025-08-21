namespace Demo.Products.Data;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        if (await context.Database.EnsureCreatedAsync())
        {
            await context.Products.AddRangeAsync(
                new Product
                {
                    Name = "Table",
                    Price = 899.99,
                    Weight = 100,
                },
                new Product
                {
                    Name = "Couch",
                    Price = 1299.50,
                    Weight = 1000,
                },
                new Product
                {
                    Name = "Chair",
                    Price = 54,
                    Weight = 50,
                });
            await context.SaveChangesAsync();
        }
    }
}