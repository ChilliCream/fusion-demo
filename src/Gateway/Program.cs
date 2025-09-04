using HotChocolate.AspNetCore;

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
    .AddHttpClient("fusion")
    .AddHeaderPropagation();

builder
    .AddGraphQLGateway()
    .AddNitro()
    .ModifyRequestOptions(o => o.CollectOperationPlanTelemetry = true);
    
var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.MapGraphQL().WithOptions(new GraphQLServerOptions { Tool = {  ServeMode = GraphQLToolServeMode.Insider } });

app.Run();
