var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.PaymentsApi, Env.Version)
    .AddNpgsqlDbContext<PaymentContext>(Env.PaymentsDb);

builder
    .AddGraphQL(Env.PaymentsApi)
    .AddDefaultSettings()
    .AddPaymentTypes()
    .InitializeOnStartup(PaymentContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
