using PuppeteerSharp;

namespace GhostCursor.PuppeteerSharp;

public static class BrowserExtensions
{
    public static ICursor<BrowserElement> CreateCursor(this IPage page, CursorOptions? options = null)
    {
        return new Cursor<PuppeteerBrowser, BrowserElement>(new PuppeteerBrowser(page), options);
    }
}
