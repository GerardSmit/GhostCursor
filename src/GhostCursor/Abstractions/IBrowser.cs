using System.Drawing;
using System.Geometry;
using System.Numerics;

namespace GhostCursor;

public interface IBrowser<TElement>
{
    Task<TElement> FindElementAsync(ElementSelector selector, CancellationToken token = default);

    Task<TElement> FindElementAsync(string selector, CancellationToken token = default);

    Task<BoundingBox> GetBoundingBox(TElement element, CancellationToken token = default);

    Task<Vector2> GetCursorAsync(CancellationToken token = default);

    Task MoveCursorToAsync(Vector2 point, CancellationToken token = default);

    Task ScrollToAsync(Vector2 point, Random random, TElement element, CancellationToken token = default);

    Task<bool> IsInViewportAsync(TElement element, CancellationToken token = default);

    Task<Size> GetViewportAsync(CancellationToken token = default);

    Task<object> EvaluateExpressionAsync(string script, CancellationToken token = default);

    Task ClickAsync(Vector2 point, int delay = 50, CancellationToken token = default);

    Task<bool> IsClickableAsync(TElement element, Vector2 point, CancellationToken token = default);

    Task AllowInputAsync(bool allow, CancellationToken token = default);

    Task TypeAsync(Random random, string text, CancellationToken token = default);

    Task<TElement> GetClickableElementAsync(TElement element, CancellationToken token = default);
}
