using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorException : Exception
{
    public CursorException()
    {
    }

    protected CursorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorException(string message) : base(message)
    {
    }

    public CursorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
