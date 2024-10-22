using System.Numerics;

namespace GhostCursor;

public interface ICursorMovement
{
    IEnumerable<Vector2> MoveTo(Random random, Vector2 start, Vector2 end, int steps);
}
