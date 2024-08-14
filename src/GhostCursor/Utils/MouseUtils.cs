using System.Geometry;
using System.Numerics;

namespace GhostCursor.Utils;

public delegate Task ScrollDeltaAsync(float deltaY);

public static class MouseUtils
{
    public static async Task ScrollDeltaAsync<TBrowser, TElement>(
        Random random,
        TBrowser browser,
        TElement element,
        ScrollDeltaAsync action,
        CancellationToken token = default)
        where TBrowser : IBrowser<TElement>
    {
        var boundingBox = await browser.GetBoundingBox(element, token);

        await ScrollDeltaAsync(random, browser, boundingBox, action, token);
    }

    public static async Task ScrollDeltaAsync(
        Random random,
        IBrowser browser,
        BoundingBox boundingBox,
        ScrollDeltaAsync action,
        CancellationToken token = default)
    {
        var isDown = boundingBox.Min.Y < 0;
        var scrollSpeed = 100f;
        var remaining = MathF.Abs(boundingBox.Min.Y);
        var cursor = await browser.GetScrollAsync(token);
        var absoluteY = cursor.Y + boundingBox.Min.Y;

        for (var i = 0; i < 10; i++)
        {
            while (remaining > 0)
            {
                var deltaY = MathF.Min(remaining, scrollSpeed);
                remaining -= deltaY;

                await action(isDown ? deltaY : -deltaY);

                await Task.Delay(random.Next(10, 50), token);
            }

            cursor = await browser.GetScrollAsync(token);
            remaining = absoluteY - cursor.Y;

            if (Math.Abs(remaining) <= 5)
            {
                return;
            }

            var isAtBottom = await browser.EvaluateExpressionAsync("JSON.stringify(window.innerHeight + window.scrollY >= document.body.offsetHeight)", token);

            if (isAtBottom is "true")
            {
                return;
            }
        }

        await browser.SetScrollAsync(cursor with { Y = absoluteY }, token);
    }
}
