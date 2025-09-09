var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ReviewsApi, Env.Version);
builder.AddNpgsqlDbContext<ReviewContext>(Env.ReviewsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ReviewsApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddReviewTypes()
    .AddPostgresSubscriptions()
    .InitializeOnStartup(ReviewContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
