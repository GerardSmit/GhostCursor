using System.Drawing;
using System.Numerics;
using GhostCursor.Utils;
using PuppeteerSharp;

namespace GhostCursor.PuppeteerSharp;

public class PuppeteerFrame(IFrame frame) : PuppeteerBrowserBase
{
    protected override IPage Page => frame.Page;

    public override async Task<Vector2> GetViewportAsync(CancellationToken token = default)
    {
        var boundingBox = JsBoundingBox.FromJson(await frame.EvaluateExpressionAsync<string>($"{JsMethods.ElementGetBoundingBox}(window.frameElement)"));

        if (boundingBox.HasValue)
        {
            return new Vector2((int)boundingBox.Value.Width, (int)boundingBox.Value.Height);
        }

        return await base.GetViewportAsync(token);
    }

    protected override Task<T> EvaluateFunctionAsync<T>(string elementInViewPort, params object[] args)
    {
        return frame.EvaluateFunctionAsync<T>(elementInViewPort, args);
    }

    public override async Task ScrollToAsync(Vector2 point, Random random, IElementHandle element, CancellationToken token = default)
    {
        var boundingBox = JsBoundingBox.FromJson(await frame.EvaluateExpressionAsync<string>($"{JsMethods.ElementGetBoundingBox}(window.frameElement)"));

        // Make sure the cursor is on top of the frame
        if (boundingBox.HasValue)
        {
            var x = boundingBox.Value.X + boundingBox.Value.Width / 2;
            var y = boundingBox.Value.Y + boundingBox.Value.Height / 2;

            await MoveCursorToAsync(new Vector2(x, y), token);
        }

        await base.ScrollToAsync(point, random, element, token);
    }

    protected override Task<T> EvaluateExpressionAsync<T>(string script)
    {
        return frame.EvaluateExpressionAsync<T>(script);
    }

    public override Task<IElementHandle> FindElementAsync(string selector, CancellationToken token = default)
    {
        return frame.QuerySelectorAsync(selector);
    }

    public override async Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default)
    {
        return (await frame.EvaluateExpressionAsync(script))?.ToString() ?? "null";
    }
}
