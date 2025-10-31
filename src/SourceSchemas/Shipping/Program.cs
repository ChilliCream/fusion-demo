var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ShippingApi, Env.Version);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ShippingApi)
    .AddNitro()
    .AddDefaultSettings(registerNodeInterface: false)
    .AddShippingTypes();

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
