using HotChocolate.Execution;

namespace Demo.Payments.Data;

public class PaymentContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
    
    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<PaymentContext>();

        if (await context.Database.EnsureCreatedAsync(cancellationToken))
        {
            await context.Payments.AddRangeAsync(
                new Payment
                {
                    Amount = 200,
                    OrderId = 1,
                    Status = PaymentStatus.Authorized,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
                new Payment
                {
                    Amount = 17,
                    OrderId = 2,
                    Status = PaymentStatus.Declined,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
                new Payment
                {
                    Amount = 17,
                    OrderId = 2,
                    Status = PaymentStatus.Authorized,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
                new Payment
                {
                    Amount = 54,
                    OrderId = 3,
                    Status = PaymentStatus.Declined,
                    CreatedAt = DateTimeOffset.UtcNow,
                });

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
