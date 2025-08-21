var builder = WebApplication.CreateBuilder(args);

builder
    .AddNpgsqlDbContext<AccountContext>(Env.AccountApi);

builder
    .AddServiceDefaults(Env.AccountApi, Env.Version);

builder
    .AddGraphQL(Env.AccountApi)
    .AddSubgraphDefaults()
    .AddTypes()
    .InitializeOnStartup(AccountContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
