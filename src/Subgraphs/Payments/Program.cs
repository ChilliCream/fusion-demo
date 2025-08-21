var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<PaymentContext>(o => o.UseSqlite("Data Source=payment.db"));

builder.AddServiceDefaults("Payments-Subgraph", Env.Version);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddUploadType()
    .AddGraphQLDefaults();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
