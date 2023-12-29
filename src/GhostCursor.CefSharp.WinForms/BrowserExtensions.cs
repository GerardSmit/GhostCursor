using CefSharp.WinForms;
using GhostCursor;

namespace GhostCursor.CefSharp;

public static class BrowserExtensions
{
    public static ICursor<ElementSelector> CreateCursor(this ChromiumWebBrowser browser, CursorOptions? options = null)
    {
        return new Cursor<CefBrowserForm, ElementSelector>(new CefBrowserForm(browser), options);
    }
}
