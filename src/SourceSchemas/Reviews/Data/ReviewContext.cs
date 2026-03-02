using HotChocolate.Execution;

namespace Demo.Reviews.Data;

public class ReviewContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<Author> Users => Set<Author>();
    
    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ReviewContext>();
        
        if (await context.Database.EnsureCreatedAsync(cancellationToken))
        {
            var ada = new Author
            {
                Name = "Ada Lovelace"
            };

            var alan = new Author
            {
                Name = "Alan Turing"
            };

            await context.Users.AddRangeAsync(ada, alan);

            await context.Reviews.AddRangeAsync(
                new Review
                {
                    Body = "Love it!",
                    Stars = 5,
                    ProductId = 1,
                    Author = ada,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new Review
                {
                    Body = "Too expensive.",
                    Stars = 1,
                    ProductId = 2,
                    Author = alan,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new Review
                {
                    Body = "Could be better.",
                    Stars = 3,
                    ProductId = 3,
                    Author = ada,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new Review
                {
                    Body = "Prefer something else.",
                    Stars = 3,
                    ProductId = 2,
                    Author = alan,
                    CreateAt = DateTimeOffset.UtcNow
                });
            
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
