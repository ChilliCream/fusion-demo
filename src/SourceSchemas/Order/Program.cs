var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.OrderApi, Env.Version)
    .AddNpgsqlDbContext<OrderContext>(Env.OrderDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.OrderApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddOrderTypes()
    .InitializeOnStartup(OrderContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
