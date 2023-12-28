namespace GhostCursor;

internal class Stopper : IAsyncDisposable
{
	private readonly ICursor _cursor;

	public Stopper(ICursor cursor)
	{
		_cursor = cursor;
	}

	public async ValueTask DisposeAsync()
	{
		await _cursor.StopAsync();
	}
}
