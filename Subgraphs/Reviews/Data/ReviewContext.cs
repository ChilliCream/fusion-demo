namespace Demo.Reviews.Data;

public class ReviewContext : DbContext
{
    public ReviewContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<User> Users => Set<User>();
}
