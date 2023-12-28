# Ghost Cursor for .NET
Simple port of [Ghost Cursor](https://github.com/Xetera/ghost-cursor/) to .NET.

## Installation
Currently only [CefSharp for WinForms](https://github.com/cefsharp/CefSharp) is supported.

Install `GhostCursor.CefSharp.WinForms` or `GhostCursor.CefSharp.WinForms.NETCore` from NuGet.

### Example
```csharp
var cef = new ChromiumWebBrowser("https://www.google.com/?hl=en");

cef.Dock = DockStyle.Fill;
Controls.Add(cef);

Task.Run(async () =>
{
    await cef.WaitForInitialLoadAsync();

    // Add "debug: true" to see the cursor in action.
    var cursor = cef.CreateCursor(debug: true);

    // Note: while the cursor is running, you can't interact with the browser.
    await using (await cursor.StartAsync())
    {
        await cursor.ClickAsync(CefElement.FromXPath("//div[text() = 'Reject all']"));
        await cursor.ClickAsync(CefElement.FromSelector("[title='Search']"));
        await cursor.TypeAsync("Hello world");
    }
});
```

![GhostCursor in action](https://i.imgur.com/1ssHq7C.gif)
