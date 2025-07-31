var builder = WebApplication.CreateBuilder(args);

builder.AddGraphQL("reviews").AddTypes();

var app = builder.Build();

app.MapGraphQL(schemaName: "reviews");

app.RunWithGraphQLCommands(args);
