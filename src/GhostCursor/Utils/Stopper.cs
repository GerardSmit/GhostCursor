namespace GhostCursor.Utils;

internal class Stopper(ICursor cursor) : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await cursor.StopAsync();
    }
}
