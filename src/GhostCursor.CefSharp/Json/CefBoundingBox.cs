using System.Text.Json.Serialization;

namespace GhostCursor;

public struct CefBoundingBox
{
	[JsonConstructor]
	public CefBoundingBox(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	[JsonPropertyName("x")]
	public float X { get; set; }

	[JsonPropertyName("y")]
	public float Y { get; set; }

	[JsonPropertyName("width")]
	public float Width { get; set; }

	[JsonPropertyName("height")]
	public float Height { get; set; }
}


public struct CefWindowScroll
{
	[JsonConstructor]
	public CefWindowScroll(float x, float y)
	{
		X = x;
		Y = y;
	}

	[JsonPropertyName("x")]
	public float X { get; set; }

	[JsonPropertyName("y")]
	public float Y { get; set; }
}