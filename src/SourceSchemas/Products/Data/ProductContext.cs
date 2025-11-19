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
                    Length = 200,
                    Width = 100,
                    Height = 75,
                    PictureFileName = "table.jpg"
                },
                new Product
                {
                    Name = "Couch",
                    Price = 1299.50,
                    Weight = 1000,
                    Length = 220,
                    Width = 90,
                    Height = 85,
                    PictureFileName = "couch.jpg"
                },
                new Product
                {
                    Name = "Chair",
                    Price = 54,
                    Weight = 50,
                    Length = 50,
                    Width = 50,
                    Height = 90,
                    PictureFileName = "chair.jpg"
                },
                new Product
                {
                    Name = "Bookshelf",
                    Price = 349.99,
                    Weight = 150,
                    Length = 120,
                    Width = 35,
                    Height = 180,
                    PictureFileName = "bookshelf.jpg"
                },
                new Product
                {
                    Name = "Desk",
                    Price = 599.00,
                    Weight = 80,
                    Length = 150,
                    Width = 75,
                    Height = 75,
                    PictureFileName = "desk.jpg"
                },
                new Product
                {
                    Name = "Bed Frame",
                    Price = 799.99,
                    Weight = 200,
                    Length = 210,
                    Width = 160,
                    Height = 40,
                    PictureFileName = "bed-frame.jpg"
                },
                new Product
                {
                    Name = "Nightstand",
                    Price = 149.50,
                    Weight = 30,
                    Length = 50,
                    Width = 40,
                    Height = 60,
                    PictureFileName = "nightstand.jpg"
                },
                new Product
                {
                    Name = "Coffee Table",
                    Price = 299.99,
                    Weight = 45,
                    Length = 120,
                    Width = 60,
                    Height = 45,
                    PictureFileName = "coffee-table.jpg"
                },
                new Product
                {
                    Name = "Dining Chair",
                    Price = 89.99,
                    Weight = 12,
                    Length = 45,
                    Width = 50,
                    Height = 95,
                    PictureFileName = "dining-chair.jpg"
                },
                new Product
                {
                    Name = "Wardrobe",
                    Price = 1499.00,
                    Weight = 250,
                    Length = 200,
                    Width = 60,
                    Height = 220,
                    PictureFileName = "wardrobe.jpg"
                },
                new Product
                {
                    Name = "TV Stand",
                    Price = 449.00,
                    Weight = 65,
                    Length = 180,
                    Width = 45,
                    Height = 55,
                    PictureFileName = "tv-stand.jpg"
                },
                new Product
                {
                    Name = "Dresser",
                    Price = 699.99,
                    Weight = 120,
                    Length = 150,
                    Width = 50,
                    Height = 90,
                    PictureFileName = "dresser.jpg"
                },
                new Product
                {
                    Name = "Armchair",
                    Price = 549.50,
                    Weight = 35,
                    Length = 85,
                    Width = 80,
                    Height = 95,
                    PictureFileName = "armchair.jpg"
                },
                new Product
                {
                    Name = "Bar Stool",
                    Price = 129.99,
                    Weight = 15,
                    Length = 40,
                    Width = 40,
                    Height = 110,
                    PictureFileName = "bar-stool.jpg"
                },
                new Product
                {
                    Name = "Sideboard",
                    Price = 899.00,
                    Weight = 140,
                    Length = 180,
                    Width = 45,
                    Height = 85,
                    PictureFileName = "sideboard.jpg"
                },
                new Product
                {
                    Name = "Console Table",
                    Price = 249.99,
                    Weight = 25,
                    Length = 120,
                    Width = 35,
                    Height = 80,
                    PictureFileName = "console-table.jpg"
                },
                new Product
                {
                    Name = "Bench",
                    Price = 179.50,
                    Weight = 22,
                    Length = 120,
                    Width = 40,
                    Height = 45,
                    PictureFileName = "bench.jpg"
                },
                new Product
                {
                    Name = "Rocking Chair",
                    Price = 399.99,
                    Weight = 28,
                    Length = 70,
                    Width = 85,
                    Height = 110,
                    PictureFileName = "rocking-chair.jpg"
                },
                new Product
                {
                    Name = "Storage Cabinet",
                    Price = 499.00,
                    Weight = 95,
                    Length = 100,
                    Width = 45,
                    Height = 150,
                    PictureFileName = "storage-cabinet.jpg"
                });

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
