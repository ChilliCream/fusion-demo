using HotChocolate.Execution.Configuration;
using Npgsql;

namespace Microsoft.Extensions.DependencyInjection;

public static class ReviewsRequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddPostgresSubscriptions(
        this IRequestExecutorBuilder builder)
        => builder.AddPostgresSubscriptions((sp, options) => options.ConnectionFactory = _ => CreateConnection(sp));

    private static ValueTask<NpgsqlConnection> CreateConnection(IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        const string configurationKey = "ConnectionStrings:" + Env.ReviewsDb;
        var connection = new NpgsqlConnection(configuration[configurationKey]);
        return new(connection);
    }
}