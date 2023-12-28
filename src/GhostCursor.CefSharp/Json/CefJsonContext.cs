using System.Text.Json.Serialization;

namespace GhostCursor;

[JsonSerializable(typeof(CefBoundingBox?))]
[JsonSerializable(typeof(CefWindowScroll))]
public partial class CefJsonContext : JsonSerializerContext;
