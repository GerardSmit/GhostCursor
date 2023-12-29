using System.Threading.Tasks;
using GhostCursor;
using GhostCursor.PuppeteerSharp;
using PuppeteerSharp;

// Initialize PuppeteerSharp
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = false
});

await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com/?hl=en");

// Create the cursor
var options = new CursorOptions
{
    Debug = true,
    DefaultSteps = 20
};

var cursor = page.CreateCursor(options);

// Search for "Hello world" with the cursor
await using (await cursor.StartAsync())
{
    await cursor.ClickAsync(ElementSelector.FromXPath("//div[text() = 'Reject all']"));
    await cursor.ClickAsync("[title='Search']");
    await cursor.TypeAsync("Hello world");
    await cursor.ClickAsync("input[value='Google Search']");
    await page.WaitForNavigationAsync();
}

// Wait for 2 seconds before closing the browser
await Task.Delay(2000);
