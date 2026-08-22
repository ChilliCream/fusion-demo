using System.Text.Json;
using System.Text.RegularExpressions;
using Aspire.Hosting.JavaScript;
using HotChocolate.Fusion.Aspire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aspire.Hosting;

/// <summary>
/// Lets a JavaScript app participate in the HotChocolate Fusion schema composition.
/// </summary>
/// <remarks>
/// The Fusion composition discovers schema-settings.json only for .NET project resources:
/// it resolves the settings directory from the resource's <see cref="IProjectMetadata"/>,
/// which only a <see cref="ProjectResource"/> carries. A JavaScript app is an
/// <see cref="ExecutableResource"/>, so its settings file is invisible to the composition
/// even though it sits right next to package.json.
///
/// This extension bridges the gap with a schema anchor: a <see cref="ProjectResource"/> that
/// exists only in the application model — no csproj on disk, never started, never shown in the
/// dashboard. Its <see cref="IProjectMetadata.ProjectPath"/> points into the JavaScript app's
/// directory, so the composition reads the real schema-settings.json from there. The anchor
/// shares the JavaScript app's <see cref="EndpointAnnotation"/> instances, so the composition
/// downloads the schema live from the app's schema endpoint and routes the gateway to the
/// app's allocated URL — the same behavior .NET source schemas get.
///
/// The app's endpoint is derived from schema-settings.json: the transport URL of the
/// environment that the referencing gateway composes against (its composition settings'
/// EnvironmentName) supplies the pinned port, the GraphQL path, and the dashboard display
/// URL. This happens in BeforeStartEvent, before the orchestrator snapshots the model, so
/// the endpoint behaves exactly as one declared in code.
///
/// The anchor is added to the model only when a composition gateway is about to start, which
/// is after the orchestrator has snapshotted the model (so DCP never tries to run it) but
/// before the composition discovers source schemas (our event subscription predates the
/// composition's, and subscriptions run in order). The composition waits for the resource
/// that carries the source schema annotation to become healthy; the anchor never runs, so we
/// wait for the JavaScript app instead and then report the anchor healthy on its behalf.
/// </remarks>
internal static partial class JavaScriptAppGraphQLSchemaExtensions
{
    /// <summary>
    /// JavaScript overload of HotChocolate.Fusion.Aspire's WithGraphQLHttpEndpoint. Overload
    /// resolution prefers this non-generic method for JavaScript app builders, so source
    /// schemas read identically in the AppHost regardless of their runtime. Path, schema
    /// path, and port default to the values in the app's schema-settings.json.
    /// </summary>
    public static IResourceBuilder<JavaScriptAppResource> WithGraphQLHttpEndpoint(
        this IResourceBuilder<JavaScriptAppResource> builder,
        string? path = null,
        string? schemaPath = null,
        string? sourceSchemaName = null,
        string portEnvironmentVariable = "PORT")
    {
        var app = builder.Resource;
        var appDirectory = Path.GetFullPath(app.WorkingDirectory, builder.ApplicationBuilder.AppHostDirectory);

        var anchor = new ProjectResource($"{app.Name}-schema");
        anchor.Annotations.Add(
            new SchemaAnchorProjectMetadata(Path.Combine(appDirectory, $"{anchor.Name}.csproj")));
        var anchorBuilder = builder.ApplicationBuilder.CreateResourceBuilder(anchor);

        // The endpoint path is only known once the settings are resolved in BeforeStartEvent;
        // the URL callback runs later still, when endpoints are allocated.
        var resolvedPath = "/graphql";
        builder.WithUrlForEndpoint("http", url => url.Url += resolvedPath);

        var eventing = builder.ApplicationBuilder.Eventing;
        var gate = new SemaphoreSlim(1, 1);

        eventing.Subscribe<BeforeStartEvent>((beforeStart, _) =>
        {
            var logger = beforeStart.Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(JavaScriptAppGraphQLSchemaExtensions).FullName!);

            var gateways = beforeStart.Model.Resources
                .Where(r => HasSchemaComposition(r) && References(r, app))
                .ToList();

            // Resolve the endpoint from the settings environment the gateway composes against.
            var settingsFile = Path.Combine(appDirectory, "schema-settings.json");
            Uri? environmentUrl = null;
            var settingsFound = File.Exists(settingsFile);

            if (settingsFound && gateways.Count > 0)
            {
                environmentUrl = ResolveEnvironmentUrl(
                    settingsFile, GetCompositionEnvironmentName(gateways[0]), logger);
            }

            resolvedPath = path
                ?? (environmentUrl?.AbsolutePath is { Length: > 1 } urlPath ? urlPath.TrimEnd('/') : "/graphql");
            var resolvedSchemaPath = schemaPath ?? $"{resolvedPath}/schema.graphql";

            // A GraphQL source schema needs an http endpoint; declare one unless the app
            // already has it, and tell the app where to bind via the given environment
            // variable. The port pin from the settings URL is cosmetic (the composition
            // routes to the allocated endpoint either way) but keeps the dashboard URL
            // predictable. BeforeStartEvent completes before the orchestrator reads the
            // model, so the endpoint behaves like one declared in code.
            if (!app.Annotations.OfType<EndpointAnnotation>().Any(e => e.UriScheme is "http" or "https"))
            {
                builder.WithHttpEndpoint(
                    port: environmentUrl?.IsLoopback is true ? environmentUrl.Port : null,
                    env: portEnvironmentVariable);
            }

            // The composition's source schema annotation, attached through the only public
            // API for it.
            anchorBuilder.WithGraphQLHttpEndpoint(
                resolvedPath, resolvedSchemaPath, sourceSchemaName: sourceSchemaName);

            if (!settingsFound)
            {
                logger.LogWarning(
                    "Skipping GraphQL schema composition for {ResourceName}: {SettingsFile} not found.",
                    app.Name, settingsFile);
                return Task.CompletedTask;
            }

            foreach (var gateway in gateways)
            {
                eventing.Subscribe<BeforeResourceStartedEvent>(gateway, async (started, cancellationToken) =>
                {
                    await gate.WaitAsync(cancellationToken);

                    try
                    {
                        // Share the app's endpoint annotations so the composition resolves the
                        // app's allocated URL through the anchor. Same instances on purpose:
                        // DCP assigns AllocatedEndpoint to them when it allocates the app's
                        // endpoints, and the anchor reflects that automatically.
                        foreach (var endpoint in app.Annotations.OfType<EndpointAnnotation>())
                        {
                            if (!anchor.Annotations.Contains(endpoint))
                            {
                                anchor.Annotations.Add(endpoint);
                            }
                        }

                        if (!beforeStart.Model.Resources.Contains(anchor))
                        {
                            beforeStart.Model.Resources.Add(anchor);
                        }

                        if (!gateway.Annotations.OfType<ResourceRelationshipAnnotation>()
                                .Any(r => ReferenceEquals(r.Resource, anchor)))
                        {
                            gateway.Annotations.Add(new ResourceRelationshipAnnotation(anchor, "Reference"));
                        }

                        var notifications =
                            started.Services.GetRequiredService<ResourceNotificationService>();

                        await notifications.WaitForResourceHealthyAsync(app.Name, cancellationToken);
                        await notifications.PublishUpdateAsync(anchor, snapshot => snapshot with
                        {
                            State = KnownResourceStates.Running,
                            IsHidden = true
                        });
                    }
                    finally
                    {
                        gate.Release();
                    }
                });
            }

            return Task.CompletedTask;
        });

        return builder;
    }

    /// <summary>
    /// Reads the transport URL template from schema-settings.json and substitutes the
    /// variables of the given environment, e.g. "{{API_URL}}" with
    /// environments.aspire.API_URL.
    /// </summary>
    private static Uri? ResolveEnvironmentUrl(string settingsFile, string environmentName, ILogger logger)
    {
        try
        {
            using var settings = JsonDocument.Parse(File.ReadAllText(settingsFile));
            var root = settings.RootElement;

            if (!root.TryGetProperty("transports", out var transports)
                || !transports.TryGetProperty("http", out var http)
                || !http.TryGetProperty("url", out var urlTemplate)
                || urlTemplate.GetString() is not { } template)
            {
                return null;
            }

            if (root.TryGetProperty("environments", out var environments)
                && environments.TryGetProperty(environmentName, out var environment))
            {
                template = EnvironmentVariablePattern().Replace(
                    template,
                    match => environment.TryGetProperty(match.Groups[1].Value, out var value)
                        ? value.GetString() ?? match.Value
                        : match.Value);
            }

            return Uri.TryCreate(template, UriKind.Absolute, out var url) ? url : null;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Could not resolve the {EnvironmentName} environment URL from {SettingsFile}.",
                environmentName, settingsFile);
            return null;
        }
    }

    private static string GetCompositionEnvironmentName(IResource gateway)
    {
        var annotation = gateway.Annotations
            .First(a => a.GetType().Name == "GraphQLSchemaCompositionAnnotation");

        return annotation.GetType().GetProperty("Settings")?.GetValue(annotation)
            is GraphQLCompositionSettings settings
                ? settings.EnvironmentName ?? "Aspire"
                : "Aspire";
    }

    private static bool HasSchemaComposition(IResource resource)
        => resource.Annotations.Any(a => a.GetType().Name == "GraphQLSchemaCompositionAnnotation");

    private static bool References(IResource resource, IResource target)
    {
        foreach (var annotation in resource.Annotations)
        {
            if (annotation is ResourceRelationshipAnnotation relationship &&
                ReferenceEquals(relationship.Resource, target))
            {
                return true;
            }

            // EndpointReferenceAnnotation is internal to Aspire.Hosting; match it the same way
            // the composition's own discovery does.
            if (annotation.GetType().Name == "EndpointReferenceAnnotation" &&
                ReferenceEquals(annotation.GetType().GetProperty("Resource")?.GetValue(annotation), target))
            {
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex("\\{\\{(\\w+)\\}\\}")]
    private static partial Regex EnvironmentVariablePattern();

    private sealed class SchemaAnchorProjectMetadata(string projectPath) : IProjectMetadata
    {
        public string ProjectPath { get; } = projectPath;
    }
}
