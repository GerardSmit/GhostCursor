using System.IO;
using System.Threading.Tasks;
using GhostCursor;
using GhostCursor.PuppeteerSharp;
using PuppeteerSharp;

// Initialize PuppeteerSharp
var path = Path.Combine(Directory.GetCurrentDirectory(), "test", "main.html");

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = false,
    Args = [
        "--disable-web-security",
        "--disable-features=IsolateOrigins,site-per-process"
    ]
});

await using var page = await browser.NewPageAsync();
await page.GoToAsync("file://" + path);

// Create the cursor
var options = new CursorOptions
{
    Debug = true,
    DefaultSteps = 20
};

// Find the frame and fill in the username
foreach (var frame in page.Frames)
{
    var element = await frame.QuerySelectorAsync("#username");

    if (element is null)
    {
        continue;
    }

    // Fill in the username
    var cursor = frame.CreateCursor(options);

    await using (await cursor.StartAsync())
    {
        await cursor.ClickAsync(element);
        await cursor.TypeAsync("Username");
    }
}

// Wait for 2 seconds before closing the browser
await Task.Delay(2000);
