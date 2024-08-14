using System.Text.Json.Serialization;

namespace GhostCursor;

[method: JsonConstructor]
internal struct JsVector2(float x, float y)
{
    [JsonPropertyName("x")] public float X { get; set; } = x;

    [JsonPropertyName("y")] public float Y { get; set; } = y;
}
