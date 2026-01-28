using Demo.Shipping.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddSingleton<ShippingCalculator>();

builder
    .AddGraphQL(Env.ShippingApi)
    .AddNitro()
    .AddDefaultSettings(registerNodeInterface: false);
    //.AddShippingTypes();

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
