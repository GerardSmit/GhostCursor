using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using GhostCursor;
using GhostCursor.CefSharp;

namespace Sandbox.CefSharp;

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

                var options = new CursorOptions
                {
                    Debug = true
                };

                var cursor = cef.CreateCursor(options);

                await using (await cursor.StartAsync())
                {
                    await cursor.ClickAsync(ElementSelector.FromCss("#checkbox-a"), token: token);
                    await cursor.ClickAsync(ElementSelector.FromCss("#checkbox-b"), token: token);

                    await cursor.ClickAsync(ElementSelector.FromCss("#input-a"), token: token);
                    await cursor.TypeAsync("Input A!!!", token: token);

                    await cursor.ClickAsync(ElementSelector.FromCss("#input-d"), token: token);
                    await cursor.TypeAsync("Input D", token: token);

                    await cursor.ClickAsync(ElementSelector.FromCss("#input-b"), token: token);
                    await cursor.TypeAsync("Input B", token: token);

                    await cursor.ClickAsync(ElementSelector.FromCss("#input-c"), token: token);
                    await cursor.TypeAsync("Input C", token: token);

                    await cursor.ClickAsync(ElementSelector.FromCss("#input-e"), token: token);
                    await cursor.TypeAsync("Input E", token: token);
                }

                cef.Reload();

                await Task.Delay(1000, token);
            }
        });
    }
}
