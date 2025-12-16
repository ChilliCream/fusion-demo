using System.Text.Json.Serialization;

namespace Demo.Gateway.Mcp;

internal sealed class McpPromptSettings
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<McpPromptSettingsArgument>? Arguments { get; set; }

    public List<McpPromptSettingsIcon>? Icons { get; set; }

    public required List<McpPromptSettingsMessage> Messages { get; set; }
}

internal sealed class McpPromptSettingsArgument
{
    public required string Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool Required { get; set; }
}

internal sealed class McpPromptSettingsIcon
{
    public required Uri Source { get; set; }

    public string? MimeType { get; set; }

    public List<string>? Sizes { get; set; }

    public string? Theme { get; set; }
}

internal sealed class McpPromptSettingsMessage
{
    public required string Role { get; set; }

    public required McpPromptSettingsMessageContent Content { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(McpPromptSettingsTextContent), "text")]
internal abstract class McpPromptSettingsMessageContent;

internal sealed class McpPromptSettingsTextContent : McpPromptSettingsMessageContent
{
    public required string Text { get; set; }
}