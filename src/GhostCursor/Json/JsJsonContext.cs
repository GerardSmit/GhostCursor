using System.Text.Json.Serialization;

namespace GhostCursor;

[JsonSerializable(typeof(JsBoundingBox?))]
[JsonSerializable(typeof(JsIsClickable))]
[JsonSerializable(typeof(JsViewport))]
[JsonSerializable(typeof(bool))]
internal partial class JsJsonContext : JsonSerializerContext;
