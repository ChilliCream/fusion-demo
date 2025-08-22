using HotChocolate.Execution;

namespace Demo.Products.Data;

public class ProductContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    
    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        if (await context.Database.EnsureCreatedAsync(cancellationToken))
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
            
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
