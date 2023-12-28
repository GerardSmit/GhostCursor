using CefSharp;
using CefSharp.WinForms;
using GhostCursor;
using GhostCursor.Extensions;

namespace Sandbox;

public class DefaultForm : Form
{
    public DefaultForm()
    {
        Width = 800;
        Height = 600;

        var cef = new ChromiumWebBrowser("https://www.google.com/?hl=en");
        cef.Dock = DockStyle.Fill;
        Controls.Add(cef);

        Task.Run(async () =>
        {   
            await cef.WaitForInitialLoadAsync();

            await Task.Delay(2000);

            var cursor = cef.CreateCursor(debug: true);

            await using (await cursor.StartAsync())
            {
                await cursor.ClickAsync(CefElement.FromXPath("//div[text() = 'Reject all']"));
                await cursor.ClickAsync(CefElement.FromSelector("[title='Search']"));
                await cursor.TypeAsync("Hello world");
            }
        });
    }
}