using Demo.Cart.Data;
using Demo.Cart.Properties;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.CartApi, Env.Version)
    .AddNpgsqlDbContext<CartContext>(Env.CartDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.CartApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddGlobalObjectIdentification(o => o.RegisterNodeInterface = false)
    .AddCartTypes()
    .AddWarmupTask(CartContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapGraphQL();
app.RunWithGraphQLCommands(args);
