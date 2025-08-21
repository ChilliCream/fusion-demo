var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ReviewsApi, Env.Version)
    .AddNpgsqlDbContext<ReviewContext>(Env.ReviewsDb);

builder
    .AddGraphQL()
    .AddSubgraphDefaults()
    .AddReviewTypes()
    .A()
    .;

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.UseWebSockets();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
