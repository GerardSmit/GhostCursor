#if NETSTANDARD2_0
namespace GhostCursor;

public class MathF
{
    public static float Floor(float x) => (float)Math.Floor(x);

    public static float Abs(float x) => Math.Abs(x);

    public static float Min(float x, float y) => Math.Min(x, y);

    public static float Ceiling(float x) => (float)Math.Ceiling(x);
}
#endif
