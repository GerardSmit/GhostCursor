namespace GhostCursor;

public readonly struct ElementSelector(ElementSelectorType type, string value)
{
    public string Value { get; } = value;

    public ElementSelectorType Type { get; } = type;

    public string ToJavaScript()
    {
        return Type switch
        {
            ElementSelectorType.JavaScript => Value,
            ElementSelectorType.Selector => $"document.querySelector('{Value.Replace("'", "\\'")}')",
            ElementSelectorType.XPath => $"document.evaluate('{Value.Replace("'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue",
            _ => throw new ArgumentOutOfRangeException(nameof(Value), Type, null)
        };
    }

    public static ElementSelector FromJavaScript(string value) => new(ElementSelectorType.JavaScript, value);

    public static ElementSelector FromCss(string value) => new(ElementSelectorType.Selector, value);

    public static ElementSelector FromXPath(string value) => new(ElementSelectorType.XPath, value);
}
