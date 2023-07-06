var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddHttpClient();
builder.Services.AddWebSocketClient();

builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromCloud(b =>
    {
        b.ApiId = Environment.GetEnvironmentVariable("BCP-API-ID") ??
            throw new InvalidOperationException("BCP-API-ID missing.");
        b.ApiKey = Environment.GetEnvironmentVariable("BCP-API-KEY") ??
            throw new InvalidOperationException("BCP-API-KEY missing.");
        b.Stage = Environment.GetEnvironmentVariable("BCP-STAGE") ??
            throw new InvalidOperationException("BCP-STAGE missing.");
    });

var app = builder.Build();

app.UseWebSockets();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
