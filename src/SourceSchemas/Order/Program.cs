var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.OrderApi, Env.Version)
    .AddNpgsqlDbContext<OrderContext>(Env.OrderDb);

builder
    .AddGraphQL(Env.OrderApi)
    .AddDefaultSettings()
    .AddOrderTypes()
    .InitializeOnStartup(OrderContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
