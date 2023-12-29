namespace GhostCursor.Utils;

public static class JsMethods
{
    //language=js
    public const string WindowSizeAsJsonObject =
        """
        JSON.stringify({
            width: window.innerWidth || document.documentElement.clientWidth,
            height: window.innerHeight || document.documentElement.clientHeight
        })
        """;

    //language=js
    public const string ElementInViewPort =
        """
        ((element) => {
            if (!element) {
                return false;
            }

            const rect = element.getBoundingClientRect();

            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
        })
        """;

    //language=js
    public const string ElementGetBoundingBox =
        """
        ((element) => {
            if (!element) {
                return "null";
            }

            const rect = element.getBoundingClientRect();

            return JSON.stringify({
                x: rect.x,
                y: rect.y,
                width: rect.width,
                height: rect.height
            });
        })
        """;

    //language=js
    public const string ElementIsClickable =
        """
        ((element, x, y) => {
            const elementAtPoint = document.elementFromPoint(x, y);

            let current = elementAtPoint;

            while (current) {
                if (current === element) {
                    return true;
                }

                current = current.parentElement;
            }

            return false;
        })
        """;

    //language=js
    public const string ElementToCssSelector =
        """
        ((element) => {
            if (element.tagName === "BODY") return "BODY";

            const names = [];
            while (element.parentElement && element.tagName !== "BODY") {
                if (element.id) {
                    names.unshift("#" + element.getAttribute("id"));
                    break;
                } else {
                    let c = 1, e = element;
                    for (; e.previousElementSibling; e = e.previousElementSibling, c++) ;
                    names.unshift(element.tagName + ":nth-child(" + c + ")");
                }
                element = element.parentElement;
            }
            return names.join(">");
        })
        """;

    //language=js
    public const string GetClickableElement =
        $$"""
        ((element) => {
            if (!element) {
                return null;
            }

            function getAlternativeElement() {
                const selector = `label[for='${element.id}']`;
                const label = document.querySelector(selector);

                if (label) {
                    return selector;
                }

                let current = element.parentElement;

                while (current) {
                    if (current.tagName === 'LABEL') {
                        return {{ElementToCssSelector}}(current);
                    }

                    current = current.parentElement;
                }

                return null;
            }

            const style = window.getComputedStyle(element);

            if (style.display === 'none' || style.visibility === 'hidden') {
                return getAlternativeElement();
            }

            const rect = element.getBoundingClientRect();

            if (rect.height === 0 || rect.width === 0) {
                return getAlternativeElement();
            }

            return null;
        })
        """;
}
