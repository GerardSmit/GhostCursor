using System.Text.Json.Serialization;

namespace GhostCursor;

[method: JsonConstructor]
internal struct JsViewport(int width, int height)
{
    [JsonPropertyName("width")] public int Width { get; set; } = width;

    [JsonPropertyName("height")] public int Height { get; set; } = height;
}
