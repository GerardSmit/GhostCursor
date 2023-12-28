using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorElementException : CursorException
{
    public CursorElementException()
    {
    }

    protected CursorElementException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorElementException(string message) : base(message)
    {
    }

    public CursorElementException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
