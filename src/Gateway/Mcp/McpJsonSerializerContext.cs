using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Gateway.Mcp;

[JsonSerializable(typeof(McpPromptSettings))]
[JsonSerializable(typeof(McpToolSettings))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
internal sealed partial class McpJsonSerializerContext : JsonSerializerContext;