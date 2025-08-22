var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.PaymentApi, Env.Version)
    .AddNpgsqlDbContext<PaymentContext>(Env.PaymentDb);

builder
    .AddGraphQL(Env.PaymentApi)
    .AddSubgraphDefaults()
    .AddPaymentTypes()
    .InitializeOnStartup(PaymentContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
