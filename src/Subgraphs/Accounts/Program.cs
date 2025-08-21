var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.AccountApi, Env.Version)
    .AddNpgsqlDbContext<AccountContext>(Env.AccountDb);

builder
    .AddGraphQL(Env.AccountApi)
    .AddSubgraphDefaults()
    .AddAccountTypes()
    .InitializeOnStartup(AccountContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
