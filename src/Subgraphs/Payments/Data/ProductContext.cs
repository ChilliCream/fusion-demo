namespace Demo.Payments.Data;

public class PaymentContext : DbContext
{
    public PaymentContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Payment> Payments => Set<Payment>();
}
