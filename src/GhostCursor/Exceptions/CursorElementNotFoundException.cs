using System.Runtime.Serialization;

namespace GhostCursor;

public class CursorElementNotFoundException : CursorElementException
{
    public CursorElementNotFoundException()
    {
    }

    protected CursorElementNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CursorElementNotFoundException(string message) : base(message)
    {
    }

    public CursorElementNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
