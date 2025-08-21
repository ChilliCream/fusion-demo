var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<AccountContext>(o => o.UseSqlite("Data Source=account.db"));

builder.AddServiceDefaults("Accounts-Subgraph", Env.Version);

builder.Services.AddGraphQLServer().AddTypes().AddGraphQLDefaults();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
