using PuppeteerSharp;

namespace GhostCursor.PuppeteerSharp;

public class PuppeteerBrowser(IPage page) : PuppeteerBrowserBase
{
    protected override IPage Page => page;

    protected override Task<T> EvaluateFunctionAsync<T>(string elementInViewPort, params object[] args)
    {
        return Page.EvaluateFunctionAsync<T>(elementInViewPort, args);
    }

    protected override Task<T> EvaluateExpressionAsync<T>(string script)
    {
        return Page.EvaluateExpressionAsync<T>(script);
    }

    public override Task<IElementHandle> FindElementAsync(string selector, CancellationToken token = default)
    {
        return Page.QuerySelectorAsync(selector);
    }

    public override async Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        return (await Page.EvaluateExpressionAsync(script))?.ToString() ?? "null";
    }
}
