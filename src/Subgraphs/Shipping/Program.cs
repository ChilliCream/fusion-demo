var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults("Shipping-Subgraph", Env.Version);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddGraphQLDefaults();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
