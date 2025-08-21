namespace Demo.Accounts.Data;

public class AccountContext : DbContext
{
    public AccountContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
