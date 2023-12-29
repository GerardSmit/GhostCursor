using CefSharp.WinForms;
using GhostCursor;

namespace GhostCursor.CefSharp;

public static class BrowserExtensions
{
    public static ICursor<BrowserElement> CreateCursor(this ChromiumWebBrowser browser, CursorOptions? options = null)
    {
        return new Cursor<CefBrowserForm, BrowserElement>(new CefBrowserForm(browser), options);
    }
}
