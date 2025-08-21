var builder = WebApplication.CreateBuilder(args);

builder
    .AddNpgsqlDbContext<AccountContext>(Env.AccountApi);

builder
    .AddServiceDefaults(Env.AccountApi, Env.Version);

builder
    .AddGraphQL(Env.AccountApi)
    .AddSubgraphDefaults()
    .AddTypes();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
