namespace Demo.Payments.Data;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentContext>();

        if (await context.Database.EnsureCreatedAsync())
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
            await context.SaveChangesAsync();
        }
    }
}
