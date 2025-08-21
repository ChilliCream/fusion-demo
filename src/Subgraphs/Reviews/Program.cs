var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ReviewsApi, Env.Version);
builder.AddRedisClient(Env.ReviewsRedis);
builder.AddNpgsqlDbContext<ReviewContext>(Env.ReviewsDb);

builder
    .AddGraphQL()
    .AddSubgraphDefaults()
    .AddReviewTypes()
    .InitializeOnStartup(ReviewContext.SeedDataAsync);

var app = builder.Build();

app.UseWebSockets();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
