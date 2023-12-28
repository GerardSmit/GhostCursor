using System.Geometry;
using System.Numerics;
using System.Text.Json;

namespace GhostCursor;

/// <summary>
/// Base class for browsers.
/// Some methods are implemented by calling JavaScript in the browser.
/// </summary>
/// <typeparam name="TElement"></typeparam>
public abstract class BrowserBase<TElement> : IBrowser<TElement>
{
	/// <summary>
	/// Converts an element to JavaScript.
	/// </summary>
	/// <param name="element">The element to convert.</param>
	/// <returns>The JavaScript representation of the element.</returns>
    protected abstract string ToJavaScript(TElement element);

    public abstract Task<TElement> FindElementAsync(string selector, CancellationToken token = default);

    public virtual async Task<BoundingBox> GetBoundingBox(TElement element, CancellationToken token = default)
    {
        var script =
            $$"""
              (function() {
              	const element = {{ToJavaScript(element)}};
              	
              	if (!element) {
              		return "null";
              	}
              	
              	const rect = element.getBoundingClientRect();
              	
              	return JSON.stringify({
              		x: rect.x,
              		y: rect.y,
              		width: Math.min(rect.width, window.innerWidth - rect.x),
              		height: Math.min(rect.height, window.innerHeight - rect.y)
              	});
              })();
              """;

        var json = await ExecuteJsAsync(script, token);
        var nullable = JsonSerializer.Deserialize(json.ToString()!, JsJsonContext.Default.NullableJsBoundingBox);

        if (nullable is not { } result)
        {
            throw new CursorElementNotFoundException($"Element '{element}' not found.");
        }

        if (result is { X: 0, Y: 0, Width: 0, Height: 0 })
        {
            throw new CursorElementNotVisibleException($"Element '{element}' not visible.");
        }

        return new BoundingBox(
            new Vector2(result.X, result.Y),
            new Vector2(result.X + result.Width, result.Y + result.Height)
        );
    }

    public abstract Task<Vector2> GetCursorAsync(CancellationToken token = default);

    public abstract Task MoveCursorToAsync(Vector2 point, CancellationToken token = default);

    public abstract Task ScrollToAsync(Vector2 point, Random random, TElement element, CancellationToken token = default);

    public abstract Task<object> ExecuteJsAsync(string script, CancellationToken token = default);

    public abstract Task ClickAsync(TElement element, Vector2 point, int delay = 50, CancellationToken token = default);

    public async Task<bool> IsClickableAsync(TElement element, Vector2 point, CancellationToken token = default)
    {
        var script =
            $$"""
              (function() {
              	const element = {{ToJavaScript(element)}};
              	
              	if (!element) {
              		return "null";
              	}
              	
              	const elementAtPoint = document.elementFromPoint({{(int)point.X}}, {{(int)point.Y}});
              	
              	let current = elementAtPoint;
              	
              	while (current) {
              		if (current === element) {
              			return JSON.stringify({
              				value: true
              			});
              		}
              		
              		current = current.parentElement;
              	}
              	
              	return JSON.stringify({
              		value: false
              	});
              })();
              """;

        var json = await ExecuteJsAsync(script, token);
        var result = JsonSerializer.Deserialize(json.ToString(), JsJsonContext.Default.JsIsClickable);

        return result.Value;
    }

    public abstract Task AllowInputAsync(bool allow, CancellationToken token = default);

    public abstract Task TypeAsync(Random random, string text, CancellationToken token = default);
}
