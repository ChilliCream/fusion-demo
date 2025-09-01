var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ReviewsApi, Env.Version);
builder.AddRedisClient(Env.ReviewsRedis);
builder.AddNpgsqlDbContext<ReviewContext>(Env.ReviewsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ReviewsApi)
    .AddDefaultSettings()
    .AddReviewTypes()
    .AddRedisSubscriptions()
    .InitializeOnStartup(ReviewContext.SeedDataAsync);

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
