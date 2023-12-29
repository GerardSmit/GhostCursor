namespace GhostCursor;

public readonly struct BrowserElement(string script)
{
    public string Script { get; } = script;

    public static BrowserElement FromJavaScript(string value) => new(value);

    public static BrowserElement FromSelector(string value) => new($"document.querySelector('{value.Replace("'", "\\'")}')");

    public static BrowserElement FromXPath(string value) => new($"document.evaluate('{value.Replace("'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue");
}
