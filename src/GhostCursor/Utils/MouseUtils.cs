namespace GhostCursor.Utils;

public delegate Task ScrollDeltaAsync(int deltaY);

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
        var attempts = 0;
        var randomOffset = random.Next(5, 30);
        var targetY = await GetYOffset(randomOffset, browser, element, token);

        while (Math.Abs(targetY) > randomOffset + 2)
        {
            if (attempts > 0)
            {
                var isAtBottom = await browser.ExecuteJsAsync("JSON.stringify(window.innerHeight + window.scrollY >= document.body.offsetHeight)", token);

                if (isAtBottom is "true")
                {
                    break;
                }
            }

            if (attempts++ > 10)
            {
                break;
            }

            var y = targetY;
            var isDown = y > 0;
            var currentY = 0;

            y = Math.Abs(y);

            while (currentY != y && !token.IsCancellationRequested)
            {
                var scrollY = Math.Min(y - currentY, 100);

                await action(isDown ? -scrollY : scrollY);

                await Task.Delay(random.Next(20, 40), token);

                currentY += scrollY;
            }

            targetY = await GetYOffset(randomOffset, browser, element, token);
        }
    }

    private static async Task<int> GetYOffset<TElement>(int randomOffset, IBrowser<TElement> browser, TElement element, CancellationToken token = default)
    {
        var boundingBox = await browser.GetBoundingBox(element, token);
        var targetY = (int)Math.Floor(boundingBox.Min.Y);
        var height = (int)(boundingBox.Max.Y - boundingBox.Min.Y);
        var viewport = await browser.GetViewportAsync(token);
        var isDown = targetY > 0;

        if (isDown)
        {
            return targetY - viewport.Height + height + randomOffset;
        }

        return targetY - randomOffset;
    }
}
