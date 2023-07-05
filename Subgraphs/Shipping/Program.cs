using static HotChocolate.WellKnownContextData;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .ConfigureSchema(b => b.SetContextData(GlobalIdSupportEnabled, 1));

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
