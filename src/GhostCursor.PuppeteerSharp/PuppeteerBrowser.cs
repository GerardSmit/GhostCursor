using System.Drawing;
using System.Numerics;
using GhostCursor.Utils;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace GhostCursor.PuppeteerSharp;

public class PuppeteerBrowser(IPage page) : BrowserBase
{
    public override Task<Size> GetViewportAsync(CancellationToken token = default)
    {
        return Task.FromResult(new Size(page.Viewport.Width, page.Viewport.Height));
    }

    public override Task<Vector2> GetCursorAsync(CancellationToken token = default)
    {
        // Not supported
        return Task.FromResult(Vector2.Zero);
    }

    public override Task AllowInputAsync(bool allow, CancellationToken token = default)
    {
        // Not supported
        return Task.CompletedTask;
    }

    public override Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
    {
        return page.Mouse.MoveAsync((int)point.X, (int)point.Y);
    }

    public override Task ScrollToAsync(Vector2 point, Random random, BrowserElement element, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, element, deltaY => page.Mouse.WheelAsync(0, deltaY * -1), token);
    }

    public override async Task<object> ExecuteJsAsync(string script, CancellationToken token = default)
    {
        var result = await page.EvaluateExpressionAsync(script);

        return result?.ToString() ?? "null";
    }

    public override Task ClickAsync(BrowserElement element, Vector2 point, int delay = 50, CancellationToken token = default)
    {
        return page.Mouse.ClickAsync((int)point.X, (int)point.Y, new ClickOptions
        {
            Delay = delay,
            Button = MouseButton.Left
        });
    }

    public override Task TypeAsync(Random random, string text, CancellationToken token = default)
    {
        return page.Keyboard.TypeAsync(text);
    }
}
