using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    private const string Production = nameof(Production); 

    public static IRequestExecutorBuilder AddDefaultSettings(
        this IRequestExecutorBuilder builder,
        bool registerNodeInterface = true)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.AddGlobalObjectIdentification(registerNodeInterface);
        builder.AddInstrumentation();
        builder.AddMutationConventions();
        builder.AddPagingArguments();
        builder.AddQueryContext();
        builder.ModifyCostOptions(x => x.EnforceCostLimits = false);

        if (Production.Equals(environmentName, StringComparison.OrdinalIgnoreCase))
        {
            builder.ExportSchemaOnStartup();
        }
        
        return builder;
    }
}
