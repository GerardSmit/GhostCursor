using System.Drawing;
using System.Geometry;
using System.Numerics;
using System.Text.Json;
using GhostCursor.Utils;

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

    public abstract Task<TElement> FindElementAsync(ElementSelector selector, CancellationToken token = default);

    public abstract Task<TElement> FindElementAsync(string selector, CancellationToken token = default);

    public virtual async Task<BoundingBox> GetBoundingBox(TElement element, CancellationToken token = default)
    {
        var script = $"{JsMethods.ElementGetBoundingBox}({ToJavaScript(element)})";
        var json = await EvaluateExpressionAsync(script, token);
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

    public virtual async Task<bool> IsInViewportAsync(TElement element, CancellationToken token = default)
    {
        var script = $"{JsMethods.ElementInViewPort}({ToJavaScript(element)})";
        var result = (bool) await EvaluateExpressionAsync(script, token);

        return result;
    }

    public virtual async Task<Size> GetViewportAsync(CancellationToken token = default)
    {
        const string script = JsMethods.WindowSizeAsJsonObject;

        var json = await EvaluateExpressionAsync(script, token);
        var result = JsonSerializer.Deserialize(json.ToString()!, JsJsonContext.Default.JsViewport);

        return new Size(result.Width, result.Height);
    }

    public abstract Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default);

    public abstract Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default);

    public virtual async Task<bool> IsClickableAsync(TElement element, Vector2 point, CancellationToken token = default)
    {
        var script = $"{JsMethods.ElementIsClickable}({ToJavaScript(element)}, {(int)point.X}, {(int)point.Y});";
        var result = (bool)await EvaluateExpressionAsync(script, token);

        return result;
    }

    public abstract Task AllowInputAsync(bool allow, CancellationToken token = default);

    public abstract Task TypeAsync(Random random, string text, CancellationToken token = default);

    public virtual async Task<TElement> GetClickableElementAsync(TElement element, CancellationToken token = default)
    {
        var script = $"{JsMethods.GetClickableElement}({ToJavaScript(element)})";
        var selector = (string?) await EvaluateExpressionAsync(script, token);

        if (selector is null)
        {
            return element;
        }

        return await FindElementAsync(selector, token);
    }
}

public abstract class BrowserBase : BrowserBase<ElementSelector>
{
    protected override string ToJavaScript(ElementSelector selector)
    {
        return selector.ToJavaScript();
    }

    public override Task<ElementSelector> FindElementAsync(string selector, CancellationToken token = default)
    {
        return Task.FromResult(ElementSelector.FromCss(selector));
    }

    public override Task<ElementSelector> FindElementAsync(ElementSelector selector, CancellationToken token = default)
    {
        return Task.FromResult(selector);
    }
}
