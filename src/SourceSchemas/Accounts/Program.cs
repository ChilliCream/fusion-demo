var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.AccountsApi, Env.Version)
    .AddNpgsqlDbContext<AccountContext>(Env.AccountsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.AccountsApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddAccountTypes()
    .AddWarmupTask(AccountContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
