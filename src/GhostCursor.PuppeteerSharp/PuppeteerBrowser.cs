using System.Drawing;
using System.Numerics;
using GhostCursor.Utils;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using BoundingBox = System.Geometry.BoundingBox;

namespace GhostCursor.PuppeteerSharp;

public class PuppeteerBrowser(IPage page) : IBrowser<IElementHandle>
{
    public Task<bool> IsInViewportAsync(IElementHandle element, CancellationToken token = default)
    {
        return page.EvaluateFunctionAsync<bool>(JsMethods.ElementInViewPort, element);
    }

    public Task<Size> GetViewportAsync(CancellationToken token = default)
    {
        return Task.FromResult(new Size(page.Viewport.Width, page.Viewport.Height));
    }

    public Task<IElementHandle> FindElementAsync(string selector, CancellationToken token = default)
    {
        return page.QuerySelectorAsync(selector);
    }

    public async Task<IElementHandle> FindElementAsync(ElementSelector selector, CancellationToken token = default)
    {
        if (selector.Type == ElementSelectorType.Selector)
        {
            return await page.QuerySelectorAsync(selector.Value);
        }

        var cssSelector = await page.EvaluateExpressionAsync<string>($"{JsMethods.ElementToCssSelector}({selector.ToJavaScript()})");
        var handle = await page.QuerySelectorAsync(cssSelector);

        return handle;
    }

    public async Task<BoundingBox> GetBoundingBox(IElementHandle element, CancellationToken token = default)
    {
        var box = await element.BoundingBoxAsync();

        return new BoundingBox(
            new Vector2((float)box.X, (float)box.Y),
            new Vector2((float)(box.X + box.Width), (float)(box.Y + box.Height))
        );
    }

    public Task<Vector2> GetCursorAsync(CancellationToken token = default)
    {
        // Not supported
        return Task.FromResult(Vector2.Zero);
    }

    public Task<bool> IsClickableAsync(IElementHandle element, Vector2 point, CancellationToken token = default)
    {
        return page.EvaluateFunctionAsync<bool>(JsMethods.ElementIsClickable, element, (int)point.X, (int)point.Y);
    }

    public Task AllowInputAsync(bool allow, CancellationToken token = default)
    {
        // Not supported
        return Task.CompletedTask;
    }

    public Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
    {
        return page.Mouse.MoveAsync((int)point.X, (int)point.Y);
    }

    public Task ScrollToAsync(Vector2 point, Random random, IElementHandle element, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, element, deltaY => page.Mouse.WheelAsync(0, deltaY * -1), token);
    }

    public async Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        var result = await page.EvaluateExpressionAsync(script);

        return result?.ToString() ?? "null";
    }

    public Task ClickAsync(IElementHandle element, Vector2 point, int delay = 50, CancellationToken token = default)
    {
        return page.Mouse.ClickAsync((int)point.X, (int)point.Y, new ClickOptions
        {
            Delay = delay,
            Button = MouseButton.Left
        });
    }

    public Task TypeAsync(Random random, string text, CancellationToken token = default)
    {
        return page.Keyboard.TypeAsync(text);
    }

    public async Task<IElementHandle> GetClickableElementAsync(IElementHandle element, CancellationToken token = default)
    {
        var selector = await page.EvaluateFunctionAsync<string>(JsMethods.GetClickableElement, element);

        if (selector is null)
        {
            return element;
        }

        return await page.QuerySelectorAsync(selector);
    }
}
