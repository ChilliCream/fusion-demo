using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Fusion.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.GatewayApi, Env.Version);

builder.Services
    .AddCors()
    .AddHeaderPropagation(c =>
    {
        c.Headers.Add("GraphQL-Preflight");
        c.Headers.Add("Authorization");
    });

builder.Services
    .AddHttpClient("Fusion")
    .AddHeaderPropagation();

builder
    .AddGraphQLGateway()
    .AddNitro(static c => c.ApiId = "QXBpCmcwMTk5MGE4ZTFkZmM3NTcxODZlZmM1OWY5OGE0YTkwZA==")
    .ModifyRequestOptions(o => o.CollectOperationPlanTelemetry = true);
    
var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.MapGraphQL().WithOptions(new GraphQLServerOptions { Tool = {  ServeMode = GraphQLToolServeMode.Insider } });

app.Run();
