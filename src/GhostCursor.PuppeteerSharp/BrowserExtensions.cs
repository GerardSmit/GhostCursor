using PuppeteerSharp;

namespace GhostCursor.PuppeteerSharp;

public static class BrowserExtensions
{
    public static ICursor<IElementHandle> CreateCursor(this IPage page, CursorOptions? options = null)
    {
        return new Cursor<PuppeteerBrowser, IElementHandle>(new PuppeteerBrowser(page), options);
    }
}
