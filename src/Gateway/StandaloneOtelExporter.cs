using System.Text;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

public static class StandaloneOtelExporter
{
    public static TracerProviderBuilder AddStandaloneOtelExporter(
        this TracerProviderBuilder builder)
        => builder.AddProcessor(sp =>
        {
            var exporterOptions = sp
                .GetRequiredService<IOptionsMonitor<StandaloneOtelExporterOptions>>()
                .CurrentValue;

            var options = new OtlpExporterOptions();
            options.ApplyOptions(exporterOptions);
            var otlpExporter = new OtlpTraceExporter(options);

            var batchOptions = new BatchExportActivityProcessorOptions();

            return new BatchActivityExportProcessor(
                otlpExporter,
                batchOptions.MaxQueueSize,
                batchOptions.ScheduledDelayMilliseconds,
                batchOptions.ExporterTimeoutMilliseconds,
                batchOptions.MaxExportBatchSize);
        });

    private static void ApplyOptions(
        this OtlpExporterOptions options,
        StandaloneOtelExporterOptions nitroOptions)
    {
        options.Protocol = OtlpExportProtocol.Grpc;
        var apiKey =
            Environment.GetEnvironmentVariable("NITRO_STANDALONE_API_KEY") ??
            Environment.GetEnvironmentVariable("BCP_STANDALONE_API_KEY");
        options.Headers = new StringBuilder()
            .Append(Headers.ApiKey)
            .Append('=')
            .Append(apiKey)
            .Append(',')
            .Append(Headers.ApiId)
            .Append('=')
            .Append(nitroOptions.ApiId)
            .Append(',')
            .Append(Headers.StageName)
            .Append('=')
            .Append(nitroOptions.Stage)
            .ToString();

        options.Endpoint = new Uri(nitroOptions.BananaCakePopUrl!);
    }
}

file static class Headers
{
    public static readonly string ApiKey = "CCC-api-key";

    public static readonly string ApiId = "CCC-api-id";

    public static readonly string StageName = "CCC-stage";
}

public sealed class StandaloneOtelExporterOptions
{
    public string? ApiId { get; set; }

    public string? Stage { get; set; }

    public string? BananaCakePopUrl { get; set; }
}
