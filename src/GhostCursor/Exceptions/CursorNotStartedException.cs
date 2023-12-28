using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorNotStartedException : CursorException
{
    public CursorNotStartedException()
    {
    }

    protected CursorNotStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorNotStartedException(string message) : base(message)
    {
    }

    public CursorNotStartedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
