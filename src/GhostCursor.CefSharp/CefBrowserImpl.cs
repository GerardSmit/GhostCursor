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

	private async Task<int> GetYOffset(int randomOffset, CefElement element, CancellationToken token = default)
	{
		var boundingBox = await GetBoundingBox(element, token);
		var targetY = (int)Math.Floor(boundingBox.Min.Y);
		var height = (int)(boundingBox.Max.Y - boundingBox.Min.Y);
		var viewport = await GetViewportAsync(token);
		var isDown = targetY > 0;

		if (isDown)
		{
			return targetY - viewport.Height + height + randomOffset;
		}

		return targetY - randomOffset;
	}

	public override async Task ScrollToAsync(Vector2 point, Random random, CefElement element, CancellationToken token = default)
	{
		var attempts = 0;
		var randomOffset = random.Next(5, 30);
		var targetY = await GetYOffset(randomOffset, element, token);

		while (Math.Abs(targetY) > randomOffset + 2)
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

			var y = targetY;
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

			targetY = await GetYOffset(randomOffset, element, token);
		}
	}

	public override async Task<object> ExecuteJsAsync(string script, CancellationToken token = default)
	{
		var response = await browser.EvaluateScriptAsync(script: script);

		return response.Result;
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
