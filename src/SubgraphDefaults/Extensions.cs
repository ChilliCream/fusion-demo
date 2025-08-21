using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IRequestExecutorBuilder AddSubgraphDefaults(this IRequestExecutorBuilder builder)
    {
        builder.AddInstrumentation();
        builder.AddGlobalObjectIdentification();
        builder.AddMutationConventions();
        builder.ModifyCostOptions(x => x.EnforceCostLimits = false);
        builder.ExportSchemaOnStartup();
        return builder;
    }
}
