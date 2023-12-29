namespace GhostCursor;

public readonly struct CefElement(string script)
{
    public string Script { get; } = script;

    public static CefElement FromJavaScript(string value) => new(value);

    public static CefElement FromSelector(string value) =>
        new($"document.querySelector('{value.Replace("'", "\\'")}')");

    public static CefElement FromXPath(string value) => new(
        $"document.evaluate('{value.Replace("'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue");
}
