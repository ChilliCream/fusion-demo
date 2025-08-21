using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Log;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder,
        string name,
        string version)
    {
        builder.Services.AddHttpContextAccessor();

        builder.ConfigureOpenTelemetry(name, version);

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler(x => x.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10));

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(
        this IHostApplicationBuilder builder,
        string name,
        string version)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(b => b.AddService(name, "Demo", version))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddEntityFrameworkCoreInstrumentation();
                tracing.AddAspNetCoreInstrumentation(o =>
                    {
                        o.RecordException = true;

                        o.EnrichWithHttpRequest = (activity, request) =>
                        {
                            var path = request.PathBase.HasValue || request.Path.HasValue
                                ? (request.PathBase + request.Path).ToString()
                                : "/";

                            // we override the display name to include the path otherwise
                            // all legacy request will be just set to {method}. This is correct
                            // otel behaviour but not useful at all
                            activity.DisplayName += $" {path}";

                            // elastic only treats a request as a "request" transaction
                            // when http.url is set
                            activity.SetTag("http.url", GetUri(request));
                        };
                        o.EnrichWithHttpResponse = (activity, _) =>
                        {
                            var rawDisplayName =
                                activity.GetCustomProperty("graphqlDisplayName");
                            if (rawDisplayName is string graphqlDisplayName &&
                                !string.IsNullOrEmpty(graphqlDisplayName))
                            {
                                activity.DisplayName = graphqlDisplayName;
                            }
                        };
                    })
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(
        this IHostApplicationBuilder builder)
    {
        var useOtlpExporter =
            !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Prometheus exporter (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
        // builder.Services.AddOpenTelemetry()
        //    .WithMetrics(metrics => metrics.AddPrometheusExporter());

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive",
                new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

            // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
            // app.MapPrometheusScrapingEndpoint();
        }

        return app;
    }

    private static string GetUri(HttpRequest request)
    {
        // this follows the suggestions from https://github.com/dotnet/aspnetcore/issues/28906
        var scheme = request.Scheme ?? string.Empty;

        // HTTP 1.0 request with NO host header would result in empty Host.
        // Use placeholder to avoid incorrect URL like "http:///"
        var host = request.Host.Value ?? "unkown";
        var pathBase = request.PathBase.Value ?? string.Empty;
        var path = request.Path.Value ?? string.Empty;
        var queryString = request.QueryString.Value ?? string.Empty;
        var length = scheme.Length + Uri.SchemeDelimiter.Length + host.Length + pathBase.Length
            + path.Length + queryString.Length;

        return string.Create(length,
            (scheme, host, pathBase, path, queryString),
            (span, parts) =>
            {
                CopyTo(ref span, parts.scheme);
                CopyTo(ref span, Uri.SchemeDelimiter);
                CopyTo(ref span, parts.host);
                CopyTo(ref span, parts.pathBase);
                CopyTo(ref span, parts.path);
                CopyTo(ref span, parts.queryString);

                static void CopyTo(ref Span<char> buffer, ReadOnlySpan<char> text)
                {
                    if (!text.IsEmpty)
                    {
                        text.CopyTo(buffer);
                        buffer = buffer.Slice(text.Length);
                    }
                }
            });
    }
}
