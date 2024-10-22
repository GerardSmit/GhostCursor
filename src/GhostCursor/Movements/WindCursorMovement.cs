using System.Numerics;
using GhostCursor;

/// <summary>
/// Wind cursor movement algorithm based on https://ben.land/post/2021/04/25/windmouse-human-mouse-movement/
/// </summary>
/// <param name="gravityForce">Magnitude of the gravitational force</param>
/// <param name="windFluctuation">Magnitude of the wind force fluctuations</param>
/// <param name="maxVelocity">Maximum step size (velocity clip threshold)</param>
/// <param name="windBehaviorChangeThreshold">Distance where wind behavior changes from random to damped</param>
public class WindCursorMovement(
    double gravityForce = 9,
    double windFluctuation = 3,
    double maxVelocity = 15,
    double windBehaviorChangeThreshold = 12
) : ICursorMovement
{
    public static WindCursorMovement Default { get; } = new();

    private static readonly double Sqrt3 = Math.Sqrt(3);
    private static readonly double Sqrt5 = Math.Sqrt(5);

    public IEnumerable<Vector2> MoveTo(Random random, Vector2 start, Vector2 end, int steps)
    {
        List<Vector2> positions = new List<Vector2>();
        double currentX = start.X, currentY = start.Y;
        double vX = 0, vY = 0, wX = 0, wY = 0;

        // Generate the full path
        while (true)
        {
            double dist = Vector2.Distance(new Vector2((float)currentX, (float)currentY), end);

            if (dist < 3)
            {
                break;
            }

            double wMag = Math.Min(windFluctuation, dist);

            if (dist >= windBehaviorChangeThreshold)
            {
                wX = wX / Sqrt3 + (2 * random.NextDouble() - 1) * wMag / Sqrt5;
                wY = wY / Sqrt3 + (2 * random.NextDouble() - 1) * wMag / Sqrt5;
            }
            else
            {
                wX /= Sqrt3;
                wY /= Sqrt3;
                if (maxVelocity < 3)
                {
                    maxVelocity = random.NextDouble() * 3 + 3;
                }
                else
                {
                    maxVelocity /= Sqrt5;
                }
            }

            vX += wX + gravityForce * (end.X - currentX) / dist;
            vY += wY + gravityForce * (end.Y - currentY) / dist;

            double vMag = Hypot(vX, vY);
            if (vMag > maxVelocity)
            {
                double vClip = maxVelocity / 2 + random.NextDouble() * maxVelocity / 2;
                vX = (vX / vMag) * vClip;
                vY = (vY / vMag) * vClip;
            }

            currentX += vX;
            currentY += vY;

            positions.Add(new Vector2((float)currentX, (float)currentY));
        }

        positions.Add(end); // Ensure the end position is included

        // Sample the path to get the desired number of steps
        int totalPositions = positions.Count;
        for (int i = 0; i < steps; i++)
        {
            int index = (int)((i * (totalPositions - 1)) / (steps - 1));
            yield return positions[index];
        }

        yield return end; // Ensure the final position is the end point
    }

    private static double Hypot(double x, double y)
    {
        return Math.Sqrt(x * x + y * y);
    }
}
