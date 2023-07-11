namespace Fab.Web.Exceptions;

public class ExceptionDto
{
    public ExceptionType Type { get; }

    public ICollection<ExceptionDetailDto> Messages { get; }

    public ExceptionDto(ExceptionType type, ICollection<ExceptionDetailDto> messages)
    {
        Type = type;
        Messages = messages;
    }

    public ExceptionDto(ExceptionType type, ExceptionDetailDto message) : this(type, new[] { message })
    {
    }

    public ExceptionDto(ExceptionType type, Exception e) : this(type, new ExceptionDetailDto(e))
    {
    }
}