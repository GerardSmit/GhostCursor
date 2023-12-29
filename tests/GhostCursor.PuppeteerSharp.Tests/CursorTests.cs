using GhostCursor.PuppeteerSharp;
using PuppeteerSharp;

namespace GhostCursor.Tests;

public class CursorTests
{
    [Fact]
    public async Task ScrollTest()
    {
        var htmlFile = Path.Combine(Directory.GetCurrentDirectory(), "Pages", "scroll.html");

        // Arrange
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        await using var page = await browser.NewPageAsync();
        await page.GoToAsync($"file:///{htmlFile.Replace("\\", "/")}");
        await page.WaitForSelectorAsync("#checkbox-a");

        // Act
        var options = new CursorOptions
        {
            Debug = true,
            DefaultSteps = 20
        };

        var cursor = page.CreateCursor(options);

        await using (await cursor.StartAsync())
        {
            await cursor.ClickAsync(BrowserElement.FromSelector("#checkbox-a"));
            await cursor.ClickAsync(BrowserElement.FromSelector("#checkbox-b"));

            await cursor.ClickAsync(BrowserElement.FromSelector("#input-a"));
            await cursor.TypeAsync("Input A");

            await cursor.ClickAsync(BrowserElement.FromSelector("#input-d"));
            await cursor.TypeAsync("Input D");

            await cursor.ClickAsync(BrowserElement.FromSelector("#input-b"));
            await cursor.TypeAsync("Input B");

            await cursor.ClickAsync(BrowserElement.FromSelector("#input-c"));
            await cursor.TypeAsync("Input C");

            await cursor.ClickAsync(BrowserElement.FromSelector("#input-e"));
            await cursor.TypeAsync("Input E");
        }

        // Assert
        Assert.True(await page.EvaluateFunctionAsync<bool>("x => x.checked", await page.QuerySelectorAsync("#checkbox-a")));
        Assert.True(await page.EvaluateFunctionAsync<bool>("x => x.checked", await page.QuerySelectorAsync("#checkbox-b")));
        Assert.Equal("Input A", await page.EvaluateFunctionAsync<string>("x => x.value", await page.QuerySelectorAsync("#input-a")));
        Assert.Equal("Input B", await page.EvaluateFunctionAsync<string>("x => x.value", await page.QuerySelectorAsync("#input-b")));
        Assert.Equal("Input C", await page.EvaluateFunctionAsync<string>("x => x.value", await page.QuerySelectorAsync("#input-c")));
        Assert.Equal("Input D", await page.EvaluateFunctionAsync<string>("x => x.value", await page.QuerySelectorAsync("#input-d")));
        Assert.Equal("Input E", await page.EvaluateFunctionAsync<string>("x => x.value", await page.QuerySelectorAsync("#input-e")));
    }
}
