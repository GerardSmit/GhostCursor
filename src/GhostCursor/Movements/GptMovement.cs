using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;

namespace GhostCursor;

public class BezierCursorMovement : ICursorMovement
{
    public static BezierCursorMovement Instance { get; } = new();

    public IEnumerable<Vector2> MoveTo(Random random, Vector2 start, Vector2 end, int steps)
    {
        // Generate control points for a cubic Bézier curve to simulate a natural, curved path.
        var controlPoint1 = GenerateControlPoint(random, start, end);
        var controlPoint2 = GenerateControlPoint(random, start, end);

        // Generate non-linear time intervals to simulate human acceleration and deceleration.
        var timeSteps = GenerateTimeSteps(steps, random);

        foreach (var t in timeSteps)
        {
            // Calculate the point on the Bézier curve at parameter t.
            var point = CalculateBezierPoint(t, start, controlPoint1, controlPoint2, end);

            // Add small random deviations to simulate hand tremor.
            point += GetRandomDeviation(random);

            yield return point;
        }
    }

    private Vector2 GenerateControlPoint(Random random, Vector2 start, Vector2 end)
    {
        // Calculate a midpoint between start and end.
        var midPoint = (start + end) / 2;

        // Determine a perpendicular vector for offsetting the control point.
        var direction = end - start;
        var perpendicular = new Vector2(-direction.Y, direction.X);
        perpendicular = Vector2.Normalize(perpendicular);

        // Randomly offset the control point along the perpendicular direction.
        var offsetDistance = (float)(random.NextDouble() - 0.5) * Vector2.Distance(start, end) / 2;
        return midPoint + perpendicular * offsetDistance;
    }

    private IEnumerable<float> GenerateTimeSteps(int steps, Random random)
    {
        var total = 0f;
        var length = steps - 1;
        var intervals = ArrayPool<float>.Shared.Rent(length);

        // Generate random intervals with slight variations.
        for (var i = 0; i < steps - 1; i++)
        {
            // Base interval is 1, with a small random variation.
            var interval = 1f + (float)(random.NextDouble() - 0.5) * 0.2f;
            intervals[i] = interval;
            total += interval;
        }

        // Normalize intervals to sum up to 1.
        for (var i = 0; i < length; i++)
        {
            intervals[i] /= total;
        }

        // Build cumulative t values.
        var t = 0f;
        yield return t;

        for (var i = 0; i < length; i++)
        {
            t += intervals[i];

            // Apply the ease-in-out function.
            var tEase = t * t * (3f - 2f * t);
            yield return tEase;
        }
    }

    private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        // Calculate the cubic Bézier curve point at parameter t.
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * p0; // First term
        point += 3 * uu * t * p1; // Second term
        point += 3 * u * tt * p2; // Third term
        point += ttt * p3;        // Fourth term

        return point;
    }

    private Vector2 GetRandomDeviation(Random random)
    {
        // Add small random deviations to each point.
        float maxDeviation = 1.0f; // Adjust this value as needed.
        float dx = (float)(random.NextDouble() * 2 - 1) * maxDeviation;
        float dy = (float)(random.NextDouble() * 2 - 1) * maxDeviation;
        return new Vector2(dx, dy);
    }
}
