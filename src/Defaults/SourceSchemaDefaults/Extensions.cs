using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IRequestExecutorBuilder AddDefaultSettings(this IRequestExecutorBuilder builder)
    {
        builder.AddInstrumentation();
        builder.AddGlobalObjectIdentification();
        builder.AddMutationConventions();
        builder.AddPagingArguments();
        builder.AddQueryContext();
        builder.ModifyCostOptions(x => x.EnforceCostLimits = false);
        builder.ExportSchemaOnStartup();
        return builder;
    }
}
