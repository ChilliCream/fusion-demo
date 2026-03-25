namespace Demo.Payments.Data;

public class PaymentContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
    
    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<PaymentContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
