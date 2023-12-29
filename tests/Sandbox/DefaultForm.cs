using CefSharp.WinForms;
using GhostCursor;

namespace Sandbox;

public class DefaultForm : Form
{
    public DefaultForm()
    {
        Width = 800;
        Height = 600;

        var cef = new ChromiumWebBrowser("browser://files/scroll.html");
        cef.Dock = DockStyle.Fill;
        Controls.Add(cef);

        Task.Run(async () =>
        {   
            await cef.WaitForInitialLoadAsync();

            var cursor = cef.CreateCursor(debug: true);

            await using (await cursor.StartAsync())
            {
                await cursor.ClickAsync(CefElement.FromSelector("#input-a"));
                await cursor.TypeAsync("Input A");

                await cursor.ClickAsync(CefElement.FromSelector("#input-d"));
                await cursor.TypeAsync("Input D");

                await cursor.ClickAsync(CefElement.FromSelector("#input-b"));
                await cursor.TypeAsync("Input B");

                await cursor.ClickAsync(CefElement.FromSelector("#input-c"));
                await cursor.TypeAsync("Input C");

                await cursor.ClickAsync(CefElement.FromSelector("#input-e"));
                await cursor.TypeAsync("Input E");
            }
        });
    }
}