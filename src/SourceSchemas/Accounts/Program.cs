var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.AccountsApi, Env.Version)
    .AddNpgsqlDbContext<AccountContext>(Env.AccountsDb);

builder
    .AddGraphQL()
    .AddDefaultSettings()
    .AddAccountTypes()
    .InitializeOnStartup(AccountContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
