using HotChocolate;
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
    .AddFileSystemConfiguration("./gateway.far")
    .ModifyRequestOptions(o => o.CollectOperationPlanTelemetry = true)
    .AddDiagnosticEventListener(sp =>
        new DebugDiagnosticListener(
            sp.GetRequiredService<IRootServiceProviderAccessor>()
                .ServiceProvider
                .GetRequiredService<ILoggerFactory>()));

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.MapGraphQL();

app.Run();
