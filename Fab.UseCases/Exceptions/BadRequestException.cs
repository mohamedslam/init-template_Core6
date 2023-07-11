using Fab.Utils.Exceptions;
using System.Net;

namespace Fab.UseCases.Exceptions;

public class BadRequestException : RestException
{
    public BadRequestException(string message) :
        base(message, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException(string message, Exception innerException) :
        base(message, HttpStatusCode.BadRequest, innerException)
    {
    }

    public BadRequestException(string message, string type) :
        base(message, type, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException(string message, string type, Exception innerException) :
        base(message, type, HttpStatusCode.BadRequest, innerException)
    {
    }
}