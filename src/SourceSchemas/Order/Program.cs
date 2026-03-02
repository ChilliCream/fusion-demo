using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.OrderApi, Env.Version)
    .AddNpgsqlDbContext<OrderContext>(Env.OrderDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.OrderApi)
    .AddDefaultSettings()
    .AddOrderTypes()
    .AddWarmupTask(OrderContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
