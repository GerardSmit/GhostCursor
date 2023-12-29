# Ghost Cursor for .NET
Simple port of [Ghost Cursor](https://github.com/Xetera/ghost-cursor/) to .NET.

## Installation
Install one of the following NuGet packages:

- For PuppeteerSharp: [`GhostCursor.PuppeteerSharp`](https://www.nuget.org/packages/GhostCursor.PuppeteerSharp/)
- For CefSharp WinForms in .NET Framework: [`GhostCursor.CefSharp.WinForms`](https://www.nuget.org/packages/GhostCursor.CefSharp.WinForms/)
- For CefSharp WinForms in .NET (Core): [`GhostCursor.CefSharp.WinForms.NetCore`](https://www.nuget.org/packages/GhostCursor.CefSharp.WinForms.NETCore/)

### Example in PuppeteerSharp
```csharp
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
    await cursor.ClickAsync(BrowserElement.FromXPath("//div[text() = 'Reject all']"));
    await cursor.ClickAsync(BrowserElement.FromSelector("[title='Search']"));
    await cursor.TypeAsync("Hello world");
    await cursor.ClickAsync(BrowserElement.FromSelector("input[value='Google Search']"));
    await page.WaitForNavigationAsync();
}

// Wait for 2 seconds before closing the browser
await Task.Delay(2000);
```

![GhostCursor in action](https://i.imgur.com/1ssHq7C.gif)
