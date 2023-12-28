using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using CefSharp.WinForms;
using GhostCursor;

namespace Sandbox;

public class CefBrowserForm : CefBrowserImpl
{
	private readonly ChromiumWebBrowser _browser;

	public CefBrowserForm(ChromiumWebBrowser browser)
		: base(browser)
	{
		_browser = browser;
	}

	public override Task<Vector2> GetCursorAsync(CancellationToken token = default)
	{
		var point = (Vector2) _browser.Invoke(() =>
		{
			var cursor = Cursor.Position;
			var point = _browser.PointToClient(cursor);
			var size = _browser.Size;

			var x = point.X;
			var y = point.Y;

			if (x > size.Width)
			{
				x = size.Width - 1;
			}
			else if (x < 0)
			{
				x = 0;
			}

			if (y > size.Height)
			{
				y = size.Height - 1;
			}
			else if (y < 0)
			{
				y = 0;
			}

			var dpi = 96f / _browser.DeviceDpi;

			if (Math.Abs(dpi - 1f) > 0.001f)
			{
				x = (int) (x * dpi);
				y = (int) (y * dpi);
			}

			return new Vector2(x, y);
		});

		return Task.FromResult(new Vector2(point.X, point.Y));
	}

	public override Task AllowInputAsync(bool allow, CancellationToken token = default)
	{
		if (_browser.InvokeRequired)
		{
			_browser.Invoke(() => _browser.Enabled = allow);
		}
		else
		{
			_browser.Enabled = allow;
		}

		return Task.CompletedTask;
	}
}
