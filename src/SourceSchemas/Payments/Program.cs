var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.PaymentsApi, Env.Version)
    .AddNpgsqlDbContext<PaymentContext>(Env.PaymentsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.PaymentsApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddPaymentTypes()
    .AddWarmupTask(PaymentContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
