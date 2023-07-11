using Fab.Utils.Exceptions;
using System.Net;

namespace Fab.UseCases.Exceptions;

public class NotFoundException : RestException
{
    public NotFoundException(string message) :
        base(message, HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string message, Exception inner) :
        base(message, HttpStatusCode.NotFound, inner)
    {
    }
}