﻿namespace GhostCursor;

public class CursorOptions
{
    /// <summary>
    /// Random number generator to use for randomizing the cursor movement and delays.
    /// </summary>
    public Random? Random { get; set; }

    /// <summary>
    /// <c>true</c> to show the cursor movement and clicks in the browser.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Default amount of steps to use for cursor movement.
    /// </summary>
    public int DefaultSteps { get; set; } = 100;

    /// <summary>
    /// Movement algorithm to use for cursor movement.
    /// </summary>
    public ICursorMovement Movement { get; set; } = BezierCursorMovement.Instance;

    /// <summary>
    /// Percentage of the time to introduce a typo in the text.
    /// </summary>
    /// <remarks>
    /// This is only supported by CEFSharp.
    /// </remarks>
    public int TypoPercentage { get; set; } = 5;
}
