using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorElementNotVisibleException : CursorElementException
{
    public CursorElementNotVisibleException()
    {
    }

    protected CursorElementNotVisibleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorElementNotVisibleException(string message) : base(message)
    {
    }

    public CursorElementNotVisibleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
