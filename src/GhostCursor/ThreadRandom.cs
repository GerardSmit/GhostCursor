namespace GhostCursor;

internal static class ThreadRandom
{
    private static readonly ThreadLocal<Random> ThreadInstance = new(() => new Random());

    public static Random Instance => ThreadInstance.Value;
}
