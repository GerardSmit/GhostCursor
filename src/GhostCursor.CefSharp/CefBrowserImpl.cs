using System.Numerics;
using CefSharp;

namespace GhostCursor;

public abstract class CefBrowserImpl(IWebBrowser browser) : BrowserBase<CefElement>
{
	protected override string ToJavaScript(CefElement element)
	{
		return element.Script;
	}

	public override Task<CefElement> FindElementAsync(string selector, CancellationToken token = default)
	{
		return Task.FromResult(CefElement.FromSelector(selector));
	}

	public override Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
	{
		var host = browser.GetBrowser().GetHost();

		host.SendMouseMoveEvent((int)point.X, (int)point.Y, false, CefEventFlags.None);

		return Task.CompletedTask;
	}

	public override async Task ScrollToAsync(Vector2 point, Random random, CefElement element, CancellationToken token = default)
	{
		var attempts = 0;
		var boundingBox = await GetBoundingBox(element, token);

		while (Math.Abs(boundingBox.Min.Y) > 2)
		{
			if (attempts > 0)
			{
				var isAtBottom = await ExecuteJsAsync("JSON.stringify(window.innerHeight + window.scrollY >= document.body.offsetHeight)", token);

				if (isAtBottom is "true")
				{
					break;
				}
			}

			if (attempts++ > 10)
			{
				break;
			}

			var y = (int)Math.Floor(boundingBox.Min.Y);
			var isDown = y > 0;
			var currentY = 0;
			var host = browser.GetBrowser().GetHost();

			y = Math.Abs(y);

			while (currentY != y && !token.IsCancellationRequested)
			{
				var scrollY = Math.Min(y - currentY, 100);

				host.SendMouseWheelEvent((int)point.X, (int)point.Y, 0, isDown ? -scrollY : scrollY, CefEventFlags.None);
				await Task.Delay(random.Next(20, 40), token);

				currentY += scrollY;
			}

			boundingBox = await GetBoundingBox(element, token);
		}
	}

	public override async Task<object> ExecuteJsAsync(string script, CancellationToken token = default)
	{
		return (await browser.EvaluateScriptAsync(script: script)).Result;
	}

	public override async Task ClickAsync(CefElement element, Vector2 point, int delay = 50, CancellationToken token = default)
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
