using OpenQA.Selenium;

namespace GhostCursor.Selenium;

public static class WebDriverExtensions
{
    public static ICursor<IWebElement> CreateCursor(this IWebDriver driver, CursorOptions? options = null)
    {
        return new Cursor<SeleniumBrowser, IWebElement>(new SeleniumBrowser(driver), options);
    }
}
