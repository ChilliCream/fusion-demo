using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ReviewsApi, Env.Version);
builder.AddNpgsqlDbContext<ReviewContext>(Env.ReviewsDb);

builder.Services.AddCors();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloakUrl = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8080";
        options.Authority = $"{keycloakUrl}/realms/fusion-demo";
        options.Audience = "graphql-api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder
    .AddGraphQL(Env.ReviewsApi)
    .AddNitro()
    .AddAuthorization()
    .AddDefaultSettings()
    .AddReviewTypes()
    .AddPostgresSubscriptions()
    .AddWarmupTask(ReviewContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
