using System.Net;

namespace Fab.Utils.Exceptions;

public class BusinessLogicException : RestException
{
    public BusinessLogicException(string message) :
        base(message, HttpStatusCode.InternalServerError)
    {
    }

    public BusinessLogicException(string message, Exception? innerException) :
        base(message, HttpStatusCode.InternalServerError, innerException)
    {
    }
}