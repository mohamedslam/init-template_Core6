using Fab.Utils.Exceptions;
using System.Net;

namespace Fab.UseCases.Exceptions;

public class ForbiddenException : RestException
{
    public ForbiddenException(string message) :
        base(message, HttpStatusCode.Forbidden)
    {
    }

    public ForbiddenException(string message, Exception inner) :
        base(message, HttpStatusCode.Forbidden, inner)
    {
    }
}