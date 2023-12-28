using CefSharp.WinForms;
using GhostCursor;

namespace Sandbox;

public static class BrowserExtensions
{
	public static ICursor<CefElement> CreateCursor(this ChromiumWebBrowser browser, Random? random = null, bool debug = false)
	{
		return new Cursor<CefBrowserImpl, CefElement>(new CefBrowserForm(browser), random, debug);
	}
}
