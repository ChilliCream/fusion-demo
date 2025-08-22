var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ShippingApi, Env.Version);

builder
    .AddGraphQL(Env.ShippingApi)
    .AddDefaultSettings(enableGlobalObjects: false)
    .AddShippingTypes()
    .InitializeOnStartup();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
