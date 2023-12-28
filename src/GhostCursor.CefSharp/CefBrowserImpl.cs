using System.Geometry;
using System.Numerics;
using System.Text.Json;
using CefSharp;

namespace GhostCursor;

public readonly struct CefElement(string script)
{
	public string Script { get; } = script;

	public static CefElement FromJavaScript(string value) => new(value);

	public static CefElement FromSelector(string value) => new($"document.querySelector('{value.Replace("'", "\\'")}')");

	public static CefElement FromXPath(string value) => new($"document.evaluate('{value.Replace("'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue");
}

public abstract class CefBrowserImpl : IBrowser<CefElement>
{
	private readonly IWebBrowser _browser;

	public CefBrowserImpl(IWebBrowser browser)
	{
		_browser = browser;
	}

	public Task<CefElement> FindElementAsync(string selector, CancellationToken token = default)
	{
		return Task.FromResult(CefElement.FromSelector(selector));
	}

	public async Task<BoundingBox> GetBoundingBox(CefElement element, CancellationToken token = default)
	{
		var script =
			$$"""
			  (function() {
			  	const element = {{element.Script}};
			  	
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

		var json = await _browser.EvaluateScriptAsync(script: script);
		var nullable = JsonSerializer.Deserialize(json.Result.ToString()!, CefJsonContext.Default.NullableCefBoundingBox);

		if (nullable is not {} result)
		{
			throw new InvalidOperationException($"Element '{element}' not found.");
		}

		return new BoundingBox(
			new Vector2(result.X, result.Y),
			new Vector2(result.X + result.Width, result.Y + result.Height)
		);
	}

	public abstract Task<Vector2> GetCursorAsync(CancellationToken token = default);

	public Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
	{
		var host = _browser.GetBrowser().GetHost();

		host.SendMouseMoveEvent((int)point.X, (int)point.Y, false, CefEventFlags.None);

		return Task.CompletedTask;
	}

	public async Task ScrollToAsync(Vector2 point, Random random, CefElement element, CancellationToken token = default)
	{
		const string script =
			"""
			 (function() {
			  return JSON.stringify({
			  	x: window.scrollX,
			  	y: window.scrollY
			  });
			 })();
			""";

		var json = await _browser.EvaluateScriptAsync(script: script);
		var result = JsonSerializer.Deserialize(json.Result.ToString()!, CefJsonContext.Default.CefWindowScroll);
		var boundingBox = await GetBoundingBox(element, token);
		var y = (int)(boundingBox.Max.Y - result.Y);
		var currentY = 0;
		var host = _browser.GetBrowser().GetHost();
		var cursor = await GetCursorAsync(token);

		while (currentY < y && !token.IsCancellationRequested)
		{
			var scrollY = Math.Min(y - currentY, 100);

			host.SendMouseWheelEvent((int)point.X, (int)point.Y, 0, -scrollY, CefEventFlags.None);
			await Task.Delay(random.Next(50, 150), token);

			currentY += scrollY;
		}
	}

	public Task ExecuteJsAsync(string script, CancellationToken token = default)
	{
		return _browser.EvaluateScriptAsync(script: script);
	}

	public async Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default)
	{
		var host = _browser.GetBrowser().GetHost();

		host.SendMouseClickEvent((int)point.X, (int)point.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
		await Task.Delay(delay, token);
		host.SendMouseClickEvent((int)point.X, (int)point.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
	}

	public abstract Task AllowInputAsync(bool allow, CancellationToken token = default);

	public async Task TypeAsync(Random random, string text, CancellationToken token = default)
	{
		var host = _browser.GetBrowser().GetHost();

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
