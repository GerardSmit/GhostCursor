using System.Geometry;
using System.Numerics;

namespace GhostCursor;

internal static class VectorUtils
{
    public static Vector2 Direction(Vector2 a, Vector2 b) => b - a;

    public static Vector2 Perpendicular(Vector2 a) => new Vector2(a.Y, -a.X);

    public static float Magnitude(Vector2 a) => (float)Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));

    public static Vector2 Unit(Vector2 a) => a / Magnitude(a);

    public static Vector2 SetMagnitude(Vector2 a, float amount) => Unit(a) * amount;

    public static float RandomNumberRange(Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static Vector2 RandomVectorOnLine(Random random, Vector2 a, Vector2 b)
    {
        var vec = Direction(a, b);
        var multiplier = (float)random.NextDouble();
        return a + (vec * multiplier);
    }

    public static (Vector2, Vector2) RandomNormalLine(Random random, Vector2 a, Vector2 b, float range)
    {
        var randMid = RandomVectorOnLine(random, a, b);
        var normalV = SetMagnitude(Perpendicular(Direction(a, randMid)), range);
        return (randMid, normalV);
    }

    public static (Vector2, Vector2) GenerateBezierAnchors(Random random, Vector2 a, Vector2 b, float spread)
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

    public static float Clamp(float target, float min, float max) => Math.Min(max, Math.Max(min, target));

    public static Vector2 Overshoot(Random random, Vector2 coordinate, float radius)
    {
        var a = random.NextDouble() * 2 * Math.PI;
        var rad = radius * (float)Math.Sqrt(random.NextDouble());
        var vector = new Vector2(rad * (float)Math.Cos(a), rad * (float)Math.Sin(a));
        return coordinate + vector;
    }

    public static Bezier BezierCurve(Random random, Vector2 start, Vector2 finish, float? overrideSpread = null)
    {
        const float min = 2;
        const float max = 200;
        var vec = Direction(start, finish);
        var length = Magnitude(vec);
        var spread = Clamp(length, min, max);
        var (anchor1, anchor2) = GenerateBezierAnchors(random, start, finish, overrideSpread ?? spread);

        return new Bezier(start, anchor1, anchor2, finish);
    }

    public static Vector2 GetRandomBoxPoint(Random random, BoundingBox box)
    {
        var x = RandomNumberRange(random, box.Min.X, box.Max.X);
        var y = RandomNumberRange(random, box.Min.Y, box.Max.Y);

        return new Vector2(x, y);
    }
}
