using System.Geometry;
using System.Numerics;

namespace GhostCursor;

public interface IBrowser
{
    Task<Vector2> GetCursorAsync(CancellationToken token = default);

    Task MoveCursorToAsync(Vector2 point, CancellationToken token = default);

    Task ScrollToAsync(Vector2 point, Random random, BoundingBox boundingBox, CancellationToken token = default);

    Task<bool> IsInViewportAsync(BoundingBox boundingBox, CancellationToken token = default);

    Task<Vector2> GetViewportAsync(CancellationToken token = default);

    Task<Vector2> GetScrollAsync(CancellationToken token = default);

    Task SetScrollAsync(Vector2 vector2, CancellationToken token = default);

    Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default);

    Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default);

    Task AllowInputAsync(bool allow, CancellationToken token = default);

    Task TypeAsync(Random random, string text, int typoPercentage = 0, CancellationToken token = default);
}

public interface IBrowser<TElement> : IBrowser
{
    Task<TElement> FindElementAsync(ElementSelector selector, CancellationToken token = default);

    Task<TElement> FindElementAsync(string selector, CancellationToken token = default);

    Task<BoundingBox> GetBoundingBox(TElement element, CancellationToken token = default);

    Task ScrollToAsync(Vector2 point, Random random, TElement element, CancellationToken token = default);

    Task<bool> IsInViewportAsync(TElement element, CancellationToken token = default);

    Task<bool> IsClickableAsync(TElement element, Vector2 point, CancellationToken token = default);

    Task<TElement> GetClickableElementAsync(TElement element, CancellationToken token = default);
}
