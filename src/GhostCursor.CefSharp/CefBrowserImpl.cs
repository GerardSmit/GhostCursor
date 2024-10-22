using System.Geometry;
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

    public override Task ScrollToAsync(Vector2 point, Random random, BoundingBox boundingBox, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, boundingBox, (deltaY) =>
        {
            browser.SendMouseWheelEvent((int)point.X, (int)point.Y, 0, (int)deltaY, CefEventFlags.None);
            return Task.CompletedTask;
        }, token);
    }

    public override async Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        var response = await browser.EvaluateScriptAsync(script: script);

        return response.Result;
    }

    public override async Task ClickAsync(Vector2 point, int delay = 50,
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

        await Task.Delay(random.Next(100, 500), token);

        var didTypo = false;

        for (var index = 0; index < text.Length; index++)
        {
            var c = text[index];

            if (token.IsCancellationRequested)
            {
                break;
            }

            if (!didTypo && random.Next(0, 100) < 8 && TypoCharacters.TryGetValue(char.ToLower(c), out var typos))
            {
                didTypo = true;

                var typo = typos[random.Next(0, typos.Length)];

                if (char.IsUpper(c))
                {
                    typo = char.ToUpper(typo);
                }

                await Type(random, host, typo, token);

                var remainingCount = text.Length - index - 1;
                var extraCount = random.Next(0, remainingCount);

                for (var i = 0; i < extraCount; i++)
                {
                    await Type(random, host, text[index + i], token);
                }

                await Task.Delay(random.Next(100, 500), token);

                for (var i = 0; i < extraCount + 1; i++)
                {
                    await Backspace(random, host, token);
                }
            }

            await Type(random, host, c, token);
        }

        await Task.Delay(random.Next(100, 500), token);
    }

    private static async Task Type(Random random, IBrowserHost host, char c, CancellationToken token = default)
    {
        var shiftDown = IsShift(c);

        if (shiftDown)
        {
            host.SendKeyEvent(new KeyEvent
            {
                Type = KeyEventType.KeyDown,
                Modifiers = CefEventFlags.ShiftDown,
                WindowsKeyCode = 0x10,
                FocusOnEditableField = true,
                IsSystemKey = false,
            });

            await Task.Delay(random.Next(10, 40), token);
        }

        host.SendKeyEvent(new KeyEvent
        {
            Type = KeyEventType.KeyDown,
            Modifiers = shiftDown ? CefEventFlags.ShiftDown : CefEventFlags.None,
            WindowsKeyCode = char.ToUpper(c),
            FocusOnEditableField = true,
            IsSystemKey = false,
        });

        host.SendKeyEvent(new KeyEvent
        {
            Type = KeyEventType.Char,
            Modifiers = shiftDown ? CefEventFlags.ShiftDown : CefEventFlags.None,
            WindowsKeyCode = c,
            FocusOnEditableField = true,
            IsSystemKey = false,
        });

        await Task.Delay(random.Next(10, 40), token);

        if (shiftDown)
        {
            host.SendKeyEvent(new KeyEvent
            {
                Type = KeyEventType.KeyUp,
                Modifiers = CefEventFlags.ShiftDown,
                WindowsKeyCode = 0x10,
                FocusOnEditableField = true,
                IsSystemKey = false,
            });

            await Task.Delay(random.Next(10, 40), token);
        }

        host.SendKeyEvent(new KeyEvent
        {
            Type = KeyEventType.KeyUp,
            Modifiers = CefEventFlags.None,
            WindowsKeyCode = char.ToUpper(c),
            FocusOnEditableField = true,
            IsSystemKey = false,
        });

        await Task.Delay(c == ' ' ? random.Next(100, 400) : random.Next(20, 60), token);
    }

    private static bool IsShift(char c)
    {
        return char.IsUpper(c);
    }

    private static async Task Backspace(Random random, IBrowserHost host, CancellationToken token = default)
    {
        host.SendKeyEvent(new KeyEvent
        {
            Type = KeyEventType.KeyDown,
            Modifiers = CefEventFlags.None,
            WindowsKeyCode = 0x08,
            FocusOnEditableField = true,
            IsSystemKey = false,
        });

        await Task.Delay(random.Next(20, 60), token);

        host.SendKeyEvent(new KeyEvent
        {
            Type = KeyEventType.KeyUp,
            Modifiers = CefEventFlags.None,
            WindowsKeyCode = 0x08,
            FocusOnEditableField = true,
            IsSystemKey = false,
        });

        await Task.Delay(random.Next(20, 60), token);
    }

    private static readonly Dictionary<char, char[]> TypoCharacters = new()
    {
        ['0'] = ['9', 'o', 'p'],
        ['1'] = ['2', 'q'],
        ['2'] = ['1', '3', 'q', 'w'],
        ['3'] = ['2', '4', 'w', 'e'],
        ['4'] = ['3', '5', 'e', 'r'],
        ['5'] = ['4', '6', 'r', 't'],
        ['6'] = ['5', '7', 't', 'y'],
        ['7'] = ['6', '8', 'y', 'u'],
        ['8'] = ['7', '9', 'u', 'i'],
        ['9'] = ['8', '0', 'i', 'o'],
        ['a'] = ['q', 's', 'z', 'w'],
        ['b'] = ['v', 'g', 'h', 'n'],
        ['c'] = ['x', 'd', 'f', 'v'],
        ['d'] = ['s', 'e', 'f', 'x', 'c', 'r'],
        ['e'] = ['w', 's', 'd', 'r', 'f'],
        ['f'] = ['d', 'r', 't', 'g', 'c', 'v', 'e'],
        ['g'] = ['f', 't', 'y', 'h', 'b', 'v'],
        ['h'] = ['g', 'y', 'u', 'j', 'n', 'b'],
        ['i'] = ['u', 'j', 'k', 'o', 'l'],
        ['j'] = ['h', 'u', 'i', 'k', 'm', 'n'],
        ['k'] = ['j', 'i', 'o', 'l', 'm'],
        ['l'] = ['k', 'o', 'p'],
        ['m'] = ['n', 'j', 'k'],
        ['n'] = ['b', 'h', 'j', 'm'],
        ['o'] = ['i', 'k', 'l', 'p'],
        ['p'] = ['o', 'l'],
        ['q'] = ['a', 'w'],
        ['r'] = ['e', 'd', 'f', 't'],
        ['s'] = ['a', 'w', 'e', 'd', 'x', 'z'],
        ['t'] = ['r', 'f', 'g', 'y'],
        ['u'] = ['y', 'h', 'j', 'i'],
        ['v'] = ['c', 'f', 'g', 'b'],
        ['w'] = ['q', 'a', 's', 'e'],
        ['x'] = ['z', 's', 'd', 'c'],
        ['y'] = ['t', 'g', 'h', 'u'],
        ['z'] = ['a', 's', 'x'],
    };
}
