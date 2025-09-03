var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ShippingApi, Env.Version);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ShippingApi)
    .AddDefaultSettings(registerNodeInterface: false)
    .AddShippingTypes()
    .InitializeOnStartup(skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
