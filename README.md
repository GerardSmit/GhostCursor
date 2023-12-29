# Ghost Cursor for .NET
Simple port of [Ghost Cursor](https://github.com/Xetera/ghost-cursor/) to .NET.

Ghost Cursor is a library for CefSharp and PuppeteerSharp that moves the mouse cursor in a more natural way.

## Installation
Install one of the following NuGet packages:

- For PuppeteerSharp: [`GhostCursor.PuppeteerSharp`](https://www.nuget.org/packages/GhostCursor.PuppeteerSharp/)
- For CefSharp WinForms in .NET Framework: [`GhostCursor.CefSharp.WinForms`](https://www.nuget.org/packages/GhostCursor.CefSharp.WinForms/)
- For CefSharp WinForms in .NET (Core): [`GhostCursor.CefSharp.WinForms.NetCore`](https://www.nuget.org/packages/GhostCursor.CefSharp.WinForms.NETCore/)

## Clicking elements
When trying to click an element, the library checks if:

1. The element is in the DOM, if not, throws `CursorElementNotFoundException`
2. The element is in the viewport, if not, it scrolls to the element.
3. The element is visible (e.g. the display is not set to none), if not, throws `CursorElementNotVisibleException`
4. The element is clickable (e.g. there is no popup blocking the element), if not, throws `CursorElementNotClickableException`.

![GhostCursor in action](https://i.imgur.com/GyBTYvL.gif)

## Example in PuppeteerSharp
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
