using System.Text.Json;
using System.Text.Json.Serialization;

namespace GhostCursor;

[method: JsonConstructor]
public struct JsBoundingBox(float x, float y, float width, float height)
{
    [JsonPropertyName("x")] public float X { get; set; } = x;

    [JsonPropertyName("y")] public float Y { get; set; } = y;

    [JsonPropertyName("width")] public float Width { get; set; } = width;

    [JsonPropertyName("height")] public float Height { get; set; } = height;

    public static JsBoundingBox? FromJson(object? json)
    {
        if (json?.ToString() is not { } value)
        {
            return null;
        }

        return JsonSerializer.Deserialize(value, JsJsonContext.Default.NullableJsBoundingBox);
    }
}
