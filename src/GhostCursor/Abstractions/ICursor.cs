namespace GhostCursor;

public interface ICursor
{
    /// <summary>
    /// Starts the cursor.
    /// </summary>
    /// <remarks>
    /// When the cursor is started, the user cannot interact with the browser.
    /// </remarks>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns an <see cref="IAsyncDisposable"/> that calls <see cref="StopAsync"/> when disposed.</returns>
    Task<IAsyncDisposable> StartAsync(CancellationToken token = default);

    /// <summary>
    /// Stops the cursor.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the cursor is stopped.</returns>
    Task StopAsync(CancellationToken token = default);

    /// <summary>
    /// Clicks the element at the specified selector.
    /// </summary>
    /// <param name="selector">The CSS selector.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the click is complete.</returns>
    /// <exception cref="CursorElementNotFoundException">Thrown when the element is not found.</exception>
    /// <exception cref="CursorElementNotVisibleException">Thrown when the element is not visible.</exception>
    /// <exception cref="CursorNotStartedException">Thrown when the cursor is not started.</exception>
    Task ClickAsync(string selector, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default);

    /// <summary>
    /// Types the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the type is complete.</returns>
    Task TypeAsync(string input, CancellationToken token = default);
}

public interface ICursor<in TElement> : ICursor
{
    /// <summary>
    /// Clicks the element at the specified selector.
    /// </summary>
    /// <param name="element">The element to click.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the click is complete.</returns>
    /// <exception cref="CursorElementNotFoundException">Thrown when the element is not found.</exception>
    /// <exception cref="CursorElementNotVisibleException">Thrown when the element is not visible.</exception>
    /// <exception cref="CursorNotStartedException">Thrown when the cursor is not started.</exception>
    Task ClickAsync(TElement element, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default);
}
