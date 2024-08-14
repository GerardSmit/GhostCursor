using System.Geometry;
using System.Numerics;

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
    Task ClickAsync(string selector, int? steps = null, TimeSpan? moveSpeed = null, CancellationToken token = default);

    /// <summary>
    /// Clicks the element at the specified selector.
    /// </summary>
    /// <param name="selector">The element to click.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the click is complete.</returns>
    /// <exception cref="CursorElementNotFoundException">Thrown when the element is not found.</exception>
    /// <exception cref="CursorElementNotVisibleException">Thrown when the element is not visible.</exception>
    /// <exception cref="CursorNotStartedException">Thrown when the cursor is not started.</exception>
    Task ClickAsync(ElementSelector selector, int? steps = null, TimeSpan? moveSpeed = null, CancellationToken token = default);

    /// <summary>
    /// Types the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the type is complete.</returns>
    Task TypeAsync(string input, CancellationToken token = default);

    /// <summary>
    /// Click the cursor at the current position.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the click is complete.</returns>
    Task ClickAsync(CancellationToken token = default);

    /// <summary>
    /// Clicks the cursor at the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the click is complete.</returns>
    Task ClickAsync(int x, int y, CancellationToken token = default);

    /// <summary>
    /// Moves the cursor to the coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="type">The position type.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the move is complete.</returns>
    Task MoveToAsync(int x, int y, int? steps = null, TimeSpan? moveSpeed = null, PositionType type = PositionType.Absolute, CancellationToken token = default);

    /// <summary>
    /// Moves the cursor to the coordinates.
    /// </summary>
    /// <param name="position">The position to move the cursor to.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="type">The position type.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task that completes when the move is complete.</returns>
    Task MoveToAsync(Vector2 position, int? steps = null, TimeSpan? moveSpeed = null, PositionType type = PositionType.Absolute, CancellationToken token = default);

    /// <summary>
    /// Moves the cursor to a random position within the bounding box.
    /// </summary>
    /// <param name="boundingBox">The bounding box to move the cursor to.</param>
    /// <param name="steps">The number of steps to move the cursor.</param>
    /// <param name="moveSpeed">The speed to move the cursor.</param>
    /// <param name="type">The position type.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a point that represents the end position of the cursor.</returns>
    Task<Vector2> MoveToAsync(BoundingBox boundingBox, int? steps = null, TimeSpan? moveSpeed = null, PositionType type = PositionType.Absolute, CancellationToken token = default);
}

public enum PositionType
{
    /// <summary>
    /// The position is relative to the current scroll position.
    /// </summary>
    Relative,

    /// <summary>
    /// The position is absolute to the page.
    /// </summary>
    Absolute
}

public interface ICursor<TElement> : ICursor
{
    /// <summary>
    /// Underlying browser.
    /// </summary>
    IBrowser<TElement> Browser { get; }

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
    Task ClickAsync(TElement element, int? steps = null, TimeSpan? moveSpeed = null, CancellationToken token = default);
}
