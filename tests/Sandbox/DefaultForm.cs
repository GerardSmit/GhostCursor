using CefSharp;
using CefSharp.WinForms;
using GhostCursor;

namespace Sandbox;

public class DefaultForm : Form
{
    private CancellationTokenSource _cts = new();

    public DefaultForm()
    {
        Width = 800;
        Height = 600;

        Closing += (_, _) =>
        {
            _cts.Cancel();
        };

        var cef = new ChromiumWebBrowser("browser://files/scroll.html");
        cef.Dock = DockStyle.Fill;
        Controls.Add(cef);

        Task.Run(async () =>
        {
            var token = _cts.Token;

            while (!token.IsCancellationRequested)
            {
                await cef.WaitForInitialLoadAsync();

                var cursor = cef.CreateCursor(debug: true);

                await using (await cursor.StartAsync())
                {
                    await cursor.ClickAsync(CefElement.FromSelector("#checkbox"), token: token);

                    await cursor.ClickAsync(CefElement.FromSelector("#input-a"), token: token);
                    await cursor.TypeAsync("Input A", token: token);

                    await cursor.ClickAsync(CefElement.FromSelector("#input-d"), token: token);
                    await cursor.TypeAsync("Input D", token: token);

                    await cursor.ClickAsync(CefElement.FromSelector("#input-b"), token: token);
                    await cursor.TypeAsync("Input B", token: token);

                    await cursor.ClickAsync(CefElement.FromSelector("#input-c"), token: token);
                    await cursor.TypeAsync("Input C", token: token);

                    await cursor.ClickAsync(CefElement.FromSelector("#input-e"), token: token);
                    await cursor.TypeAsync("Input E", token: token);
                }

                cef.Reload();

                await Task.Delay(1000, token);
            }
        });
    }
}
