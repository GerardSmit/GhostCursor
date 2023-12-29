using System.Numerics;
using CefSharp;
using GhostCursor.Utils;

namespace GhostCursor.CefSharp;

public abstract class CefBrowserImpl(IWebBrowser browser) : BrowserBase
{
    public override Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
    {
        var host = browser.GetBrowser().GetHost();

        host.SendMouseMoveEvent((int)point.X, (int)point.Y, false, CefEventFlags.None);

        return Task.CompletedTask;
    }

    public override Task ScrollToAsync(Vector2 point, Random random, ElementSelector selector, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, selector, (deltaY) =>
        {
            browser.SendMouseWheelEvent((int)point.X, (int)point.Y, 0, deltaY, CefEventFlags.None);
            return Task.CompletedTask;
        }, token);
    }

    public override async Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        var response = await browser.EvaluateScriptAsync(script: script);

        return response.Result;
    }

    public override async Task ClickAsync(ElementSelector selector, Vector2 point, int delay = 50,
        CancellationToken token = default)
    {
        var host = browser.GetBrowser().GetHost();

        host.SendMouseClickEvent((int)point.X, (int)point.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
        await Task.Delay(delay, token);
        host.SendMouseClickEvent((int)point.X, (int)point.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
    }

    public override async Task TypeAsync(Random random, string text, CancellationToken token = default)
    {
        var host = browser.GetBrowser().GetHost();

        foreach (var c in text)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            host.SendKeyEvent(new KeyEvent
            {
                Type = KeyEventType.Char,
                Modifiers = CefEventFlags.None,
                WindowsKeyCode = c,
                FocusOnEditableField = true,
                IsSystemKey = false,
            });

            await Task.Delay(random.Next(20, 40), token);
        }
    }
}
