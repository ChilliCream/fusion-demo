var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.InventoryApi, Env.Version)
    .AddNpgsqlDbContext<InventoryContext>(Env.InventoryDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.InventoryApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddInventoryTypes()
    .AddWarmupTask(InventoryContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
