var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<AccountContext>(
        o => o.UseSqlite("Data Source=account.db"));

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddGlobalObjectIdentification()
    .RegisterDbContext<AccountContext>();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
