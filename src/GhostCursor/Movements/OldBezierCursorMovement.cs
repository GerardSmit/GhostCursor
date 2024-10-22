using System.Geometry;
using System.Numerics;

namespace GhostCursor;

public class OldBezierCursorMovement : ICursorMovement
{
    public static OldBezierCursorMovement Instance { get; } = new();

    public IEnumerable<Vector2> MoveTo(Random random, Vector2 start, Vector2 end, int steps)
    {
        var bezier = BezierCurve(random, start, end);

        for (var i = 0; i < steps; i++)
        {
            yield return bezier.Position(i / (float)steps);
        }
    }

    private static Vector2 Direction(Vector2 a, Vector2 b) => b - a;

    private static Vector2 Perpendicular(Vector2 a) => new Vector2(a.Y, -a.X);

    private static float Magnitude(Vector2 a) => (float)Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));

    private static Vector2 Unit(Vector2 a) => a / Magnitude(a);

    private static Vector2 SetMagnitude(Vector2 a, float amount) => Unit(a) * amount;

    private static float RandomNumberRange(Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    private static Vector2 RandomVectorOnLine(Random random, Vector2 a, Vector2 b)
    {
        var vec = Direction(a, b);
        var multiplier = (float)random.NextDouble();
        return a + (vec * multiplier);
    }

    private static (Vector2, Vector2) RandomNormalLine(Random random, Vector2 a, Vector2 b, float range)
    {
        var randMid = RandomVectorOnLine(random, a, b);
        var normalV = SetMagnitude(Perpendicular(Direction(a, randMid)), range);
        return (randMid, normalV);
    }

    private static (Vector2, Vector2) GenerateBezierAnchors(Random random, Vector2 a, Vector2 b, float spread)
    {
        var side = random.Next(0, 2) == 1 ? 1 : -1;
        var anchor1 = GenerateBezierAnchors(random, a, b, spread, side);
        var anchor2 = GenerateBezierAnchors(random, a, b, spread, side);

        return anchor1.X < anchor2.X ? (anchor1, anchor2) : (anchor2, anchor1);
    }

    private static Vector2 GenerateBezierAnchors(Random random, Vector2 a, Vector2 b, float spread, int side)
    {
        var (randMid, normalV) = RandomNormalLine(random, a, b, spread);
        var choice = normalV * side;
        return RandomVectorOnLine(random, randMid, randMid + choice);
    }

    private static float Clamp(float target, float min, float max) => Math.Min(max, Math.Max(min, target));

    private static Bezier BezierCurve(Random random, Vector2 start, Vector2 finish, float? overrideSpread = null)
    {
        const float min = 2;
        const float max = 200;
        var vec = Direction(start, finish);
        var length = Magnitude(vec);
        var spread = Clamp(length, min, max);
        var (anchor1, anchor2) = GenerateBezierAnchors(random, start, finish, overrideSpread ?? spread);

        return new Bezier(start, anchor1, anchor2, finish);
    }
}
