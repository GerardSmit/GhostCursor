using System.Numerics;
using System.Text.Json;

namespace GhostCursor.Utils;

public class BrowserUtils
{
    public static async Task<Vector2> GetScrollAsync(IBrowser browser, CancellationToken token = default)
    {
        const string script = JsMethods.WindowScrollAsJsonObject;

        var json = await browser.EvaluateExpressionAsync(script, token);
        var result = JsonSerializer.Deserialize(json.ToString()!, JsJsonContext.Default.JsVector2);

        return new Vector2(result.X, result.Y);
    }

    public static Task SetScrollAsync(IBrowser browserBase, Vector2 vector2, CancellationToken token = default)
    {
        FormattableString script = $"{JsMethods.SetWindowScroll}({{x: {vector2.X}, y: {vector2.Y}}})";

        return browserBase.EvaluateExpressionAsync(FormattableString.Invariant(script), token);
    }

    public static async Task<Vector2> GetViewportAsync(IBrowser browserBase, CancellationToken token = default)
    {
        const string script = JsMethods.WindowSizeAsJsonObject;

        var json = await browserBase.EvaluateExpressionAsync(script, token);
        var result = JsonSerializer.Deserialize(json.ToString()!, JsJsonContext.Default.JsViewport);

        return new Vector2(result.Width, result.Height);
    }
}
