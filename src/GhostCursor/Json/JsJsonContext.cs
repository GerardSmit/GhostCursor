using System.Text.Json.Serialization;

namespace GhostCursor;

[JsonSerializable(typeof(JsBoundingBox?))]
[JsonSerializable(typeof(JsIsClickable))]
[JsonSerializable(typeof(JsViewport))]
[JsonSerializable(typeof(JsVector2))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(string))]
internal partial class JsJsonContext : JsonSerializerContext;
