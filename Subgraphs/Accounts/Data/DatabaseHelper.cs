namespace Demo.Accounts.Data;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountContext>();
        if(await context.Database.EnsureCreatedAsync())
        {
            await context.Users.AddRangeAsync(
                new User
                {
                    Name = "Ada Lovelace",
                    Birthdate = new DateTime(1815, 12, 10),
                    Username = "@ada"
                },
                new User
                {
                    Name = "Alan Turing",
                    Birthdate = new DateTime(1912, 06, 23),
                    Username = "@alan"
                });
            await context.SaveChangesAsync();
        }
    }

}