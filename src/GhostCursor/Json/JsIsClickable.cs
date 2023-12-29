using System.Text.Json.Serialization;

namespace GhostCursor;

[method: JsonConstructor]
internal struct JsIsClickable(bool value)
{
    [JsonPropertyName("value")] public bool Value { get; set; } = value;
}
