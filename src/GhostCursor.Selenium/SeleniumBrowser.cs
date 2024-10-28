using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using GhostCursor.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using BoundingBox = System.Geometry.BoundingBox;

namespace GhostCursor.Selenium;

public class SeleniumBrowser(IWebDriver driver) : IBrowser<IWebElement>
{
    private readonly IJavaScriptExecutor _executor = (IJavaScriptExecutor)driver;

    public Task<bool> IsInViewportAsync(BoundingBox boundingBox, CancellationToken token = default)
    {
        FormattableString script = $"return {JsMethods.ElementInViewPort}({{top: {boundingBox.Min.Y}, left: {boundingBox.Min.X}, bottom: {boundingBox.Max.Y}, right: {boundingBox.Max.X}}})";
        var result = _executor.ExecuteScript(FormattableString.Invariant(script));

        return Task.FromResult((bool)result);
    }

    public Task<bool> IsInViewportAsync(IWebElement element, CancellationToken token = default)
    {
        var result = _executor.ExecuteScript($"return {JsMethods.ElementInViewPort}(arguments[0])", element);

        return Task.FromResult((bool)result);
    }

    public Task<Vector2> GetViewportAsync(CancellationToken token = default)
    {
        var size = driver.Manage().Window.Size;
        return Task.FromResult(new Vector2(size.Width, size.Height));
    }

    public Task<Vector2> GetScrollAsync(CancellationToken token = default)
    {
        var point = driver.Manage().Window.Position;

        return Task.FromResult(new Vector2(point.X, point.Y));
    }

    public Task SetScrollAsync(Vector2 vector2, CancellationToken token = default)
    {
        driver.Manage().Window.Position = new Point((int)vector2.X, (int)vector2.Y);
        return Task.CompletedTask;
    }

    public Task<IWebElement> FindElementAsync(string selector, CancellationToken token = default)
    {
        return Task.FromResult(driver.FindElement(By.CssSelector(selector)));
    }

    public Task<IWebElement> FindElementAsync(ElementSelector selector, CancellationToken token = default)
    {
        if (selector.Type == ElementSelectorType.Selector)
        {
            return Task.FromResult(driver.FindElement(By.CssSelector(selector.Value)));
        }

        if (selector.Type == ElementSelectorType.XPath)
        {
            return Task.FromResult(driver.FindElement(By.XPath(selector.Value)));
        }

        var cssSelector = (string) _executor.ExecuteScript($"return {JsMethods.ElementToCssSelector}({selector.ToJavaScript()})");
        var handle = driver.FindElement(By.CssSelector(cssSelector));

        return Task.FromResult(handle);
    }

    public Task<BoundingBox> GetBoundingBox(IWebElement element, CancellationToken token = default)
    {
        var location = element.Location;
        var size = element.Size;

        return Task.FromResult(new BoundingBox(
            new Vector2(location.X, location.Y),
            new Vector2(location.X + size.Width, location.Y + size.Height)
        ));
    }

    public Task<Vector2> GetCursorAsync(CancellationToken token = default)
    {
        // Not supported
        return Task.FromResult(Vector2.Zero);
    }

    public Task<bool> IsClickableAsync(IWebElement element, Vector2 point, CancellationToken token = default)
    {
        return Task.FromResult((bool) _executor.ExecuteScript(
            $"return {JsMethods.ElementIsClickable}(arguments[0], arguments[1], arguments[2])",
            element,
            (int)point.X,
            (int)point.Y));
    }

    public Task AllowInputAsync(bool allow, CancellationToken token = default)
    {
        // Not supported
        return Task.CompletedTask;
    }

    public Task MoveCursorToAsync(Vector2 point, CancellationToken token = default)
    {
        var x = (int)point.X;
        var y = (int)point.Y;
        var viewport = driver.Manage().Window.Size;

        if (x < 0 || x > viewport.Width || y < 0 || y > viewport.Height)
        {
            return Task.CompletedTask;
        }

        new Actions(driver, TimeSpan.Zero)
            .MoveToLocation(x, y)
            .Perform();

        return Task.CompletedTask;
    }
    public Task ScrollToAsync(Vector2 point, Random random, BoundingBox boundingBox, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, boundingBox, deltaY =>
        {
            new Actions(driver, TimeSpan.Zero).ScrollByAmount(0, (int)deltaY).Perform();
            return Task.CompletedTask;
        }, token);
    }

    public Task ScrollToAsync(Vector2 point, Random random, IWebElement element, CancellationToken token = default)
    {
        return MouseUtils.ScrollDeltaAsync(random, this, element, deltaY =>
        {
            new Actions(driver, TimeSpan.Zero).ScrollByAmount(0, (int)deltaY).Perform();
            return Task.CompletedTask;
        }, token);
    }

    public Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        return Task.FromResult(_executor.ExecuteScript($"return {script}"));
    }

    public Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default)
    {
        new Actions(driver, TimeSpan.Zero)
            .MoveToLocation((int)point.X, (int)point.Y)
            .Click()
            .Perform();

        return Task.CompletedTask;
    }

    public Task TypeAsync(Random random, string text, int typoPercentage = 0, CancellationToken token = default)
    {
        new Actions(driver, TimeSpan.Zero)
            .SendKeys(text)
            .Perform();

        return Task.CompletedTask;
    }

    public Task<IWebElement> GetClickableElementAsync(IWebElement element, CancellationToken token = default)
    {
        var selector = (string?)_executor.ExecuteScript($"return {JsMethods.GetClickableElement}(arguments[0])", element);

        if (selector is null)
        {
            return Task.FromResult(element);
        }

        return FindElementAsync(selector, token);
    }
}
