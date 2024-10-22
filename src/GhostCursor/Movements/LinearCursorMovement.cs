using System.Numerics;

namespace GhostCursor;

public class LinearCursorMovement : ICursorMovement
{
    public static LinearCursorMovement Instance { get; } = new();

    public IEnumerable<Vector2> MoveTo(Random random, Vector2 start, Vector2 end, int steps)
    {
        var direction = end - start;
        var step = direction / steps;

        for (var i = 0; i < steps; i++)
        {
            yield return start + step * i;
        }
    }
}
