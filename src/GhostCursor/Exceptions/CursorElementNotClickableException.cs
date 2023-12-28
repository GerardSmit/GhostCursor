using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorElementNotClickableException : CursorElementException
{
    public CursorElementNotClickableException()
    {
    }

    protected CursorElementNotClickableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorElementNotClickableException(string message) : base(message)
    {
    }

    public CursorElementNotClickableException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
