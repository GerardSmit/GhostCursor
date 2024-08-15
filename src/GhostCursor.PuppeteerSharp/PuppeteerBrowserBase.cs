using System.Drawing;
using System.Numerics;
using GhostCursor.Utils;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using BoundingBox = System.Geometry.BoundingBox;

namespace GhostCursor.PuppeteerSharp;

public abstract class PuppeteerBrowserBase : IBrowser<IElementHandle>
{
    protected abstract IPage Page { get; }

    protected abstract Task<T> EvaluateFunctionAsync<T>(string elementInViewPort, params object[] args);

    protected abstract Task<T> EvaluateExpressionAsync<T>(string script);

    public abstract Task<IElementHandle> FindElementAsync(string selector, CancellationToken token = default);

    public Task<bool> IsInViewportAsync(BoundingBox boundingBox, CancellationToken token = default)
    {
        FormattableString script = $"{JsMethods.ElementInViewPort}({{top: {boundingBox.Min.Y}, left: {boundingBox.Min.X}, bottom: {boundingBox.Max.Y}, right: {boundingBox.Max.X}}})";

        return EvaluateExpressionAsync<bool>(FormattableString.Invariant(script));
    }

    public Task<bool> IsInViewportAsync(IElementHandle element, CancellationToken token = default)
    {
        return EvaluateFunctionAsync<bool>(JsMethods.ElementInViewPort, element);
    }

    public virtual Task<Vector2> GetViewportAsync(CancellationToken token = default)
    {
        return Page.Viewport is null
            ? BrowserUtils.GetViewportAsync(this, token)
            : Task.FromResult(new Vector2(Page.Viewport.Width, Page.Viewport.Height));
    }

    public Task<Vector2> GetScrollAsync(CancellationToken token = default)
    {
        return BrowserUtils.GetScrollAsync(this, token);
    }

    public Task SetScrollAsync(Vector2 vector2, CancellationToken token = default)
    {
        return BrowserUtils.SetScrollAsync(this, vector2, token);
    }

    public async Task<IElementHandle> FindElementAsync(ElementSelector selector, CancellationToken token = default)
    {
        if (selector.Type == ElementSelectorType.Selector)
        {
            return await FindElementAsync(selector.Value, token);
        }

        var cssSelector = await EvaluateExpressionAsync<string>($"{JsMethods.ElementToCssSelector}({selector.ToJavaScript()})");
        var handle = await FindElementAsync(cssSelector, token);

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
        return EvaluateFunctionAsync<bool>(JsMethods.ElementIsClickable, element, (int)point.X, (int)point.Y);
    }

    public Task AllowInputAsync(bool allow, CancellationToken token = default)
    {
        // Not supported
        return Task.CompletedTask;
    }

    public Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
    {
        return Page.Mouse.MoveAsync((int)point.X, (int)point.Y);
    }

    public async Task ScrollToAsync(Vector2 point, Random random, BoundingBox boundingBox, CancellationToken token = default)
    {
        await MouseUtils.ScrollDeltaAsync(random, this, boundingBox, deltaY => Page.Mouse.WheelAsync(0, (decimal)(deltaY * -1)), token);
    }

    public virtual async Task ScrollToAsync(Vector2 point, Random random, IElementHandle element, CancellationToken token = default)
    {
        await MouseUtils.ScrollDeltaAsync(random, this, element, deltaY => Page.Mouse.WheelAsync(0, (decimal)(deltaY * -1)), token);
    }

    public abstract Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default);

    public Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default)
    {
        return Page.Mouse.ClickAsync((int)point.X, (int)point.Y, new ClickOptions
        {
            Delay = delay,
            Button = MouseButton.Left
        });
    }

    public Task TypeAsync(Random random, string text, CancellationToken token = default)
    {
        return Page.Keyboard.TypeAsync(text);
    }

    public async Task<IElementHandle> GetClickableElementAsync(IElementHandle element, CancellationToken token = default)
    {
        var selector = await EvaluateFunctionAsync<string>(JsMethods.GetClickableElement, element);

        if (selector == null!)
        {
            return element;
        }

        return await Page.QuerySelectorAsync(selector);
    }
}
