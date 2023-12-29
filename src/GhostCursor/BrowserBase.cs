using System.Drawing;
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
              		width: rect.width,
              		height: rect.height
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

    public abstract Task ScrollToAsync(Vector2 point, Random random, TElement element,
        CancellationToken token = default);

    public async Task<bool> IsInViewportAsync(TElement element, CancellationToken token = default)
    {
        var script =
            $$"""
              (function() {
              	const element = {{ToJavaScript(element)}};
              	
              	if (!element) {
              		return "false";
              	}
              	
              	const rect = element.getBoundingClientRect();
              
              	return (
              		rect.top >= 0 &&
              		rect.left >= 0 &&
              		rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
              		rect.right <= (window.innerWidth || document.documentElement.clientWidth)
              	) ? "true" : "false";
              })();
              """;

        var json = await ExecuteJsAsync(script, token);

        return json.ToString() == "true";
    }

    public async Task<Size> GetViewportAsync(CancellationToken token = default)
    {
        var script =
            """
            JSON.stringify({
            	width: window.innerWidth || document.documentElement.clientWidth,
            	height: window.innerHeight || document.documentElement.clientHeight
            });
            """;

        var json = await ExecuteJsAsync(script, token);
        var result = JsonSerializer.Deserialize(json.ToString()!, JsJsonContext.Default.JsViewport);

        return new Size(result.Width, result.Height);
    }

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

    public async Task<TElement> GetClickableElementAsync(TElement element, CancellationToken token = default)
    {
        var script =
            $$"""
              (function() {
              	const element = {{ToJavaScript(element)}};
              	
              	if (!element) {
              		return "null";
              	}
              	
              	function getAlternativeElement() {
              		const selector = `label[for='${element.id}']`;
              		const label = document.querySelector(selector);
              		
              		return label ? selector : "null";
              	}
              	
              	const style = window.getComputedStyle(element);
              	
              	if (style.display === 'none' || style.visibility === 'hidden') {
              		return getAlternativeElement();
              	}
              
              	const rect = element.getBoundingClientRect();
              
              	if (rect.height === 0 || rect.width === 0) {
              		return getAlternativeElement();
              	}
              	
              	return "null";
              })();
              """;

        var json = await ExecuteJsAsync(script, token);

        return json.ToString() switch
        {
            "null" => element,
            { } selector => await FindElementAsync(selector, token),
        };
    }
}
