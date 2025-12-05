using System.Collections.Immutable;

namespace Demo.Gateway.Mcp;

internal sealed class McpToolSettings
{
    public string? Title { get; set; }

    public List<McpToolSettingsIcon>? Icons { get; set; }

    public McpToolSettingsAnnotations? Annotations { get; set; }

    public McpToolSettingsOpenAiComponent? OpenAiComponent { get; set; }
}

internal sealed class McpToolSettingsIcon
{
    public required Uri Source { get; set; }

    public string? MimeType { get; set; }

    public List<string>? Sizes { get; set; }

    public string? Theme { get; set; }
}

internal sealed class McpToolSettingsAnnotations
{
    public bool? DestructiveHint { get; set; }

    public bool? IdempotentHint { get; set; }

    public bool? OpenWorldHint { get; set; }
}

internal sealed class McpToolSettingsComponent
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Domain { get; set; }

    public bool? PrefersBorder { get; set; }

    public McpToolSettingsCsp? Csp { get; set; }
}

internal sealed class McpToolSettingsOpenAiComponent
{
    public string? Description { get; set; }

    public string? Domain { get; set; }

    public bool? PrefersBorder { get; set; }

    public string? ToolInvokingStatusText { get; set; }

    public string? ToolInvokedStatusText { get; set; }

    public bool? AllowToolCalls { get; set; }

    public McpToolSettingsCsp? Csp { get; set; }
}

internal sealed class McpToolSettingsCsp
{
    public ImmutableArray<string>? ConnectDomains { get; set; }

    public ImmutableArray<string>? ResourceDomains { get; set; }
}