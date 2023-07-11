using Fab.Utils.Exceptions;
using FluentValidation;
using System.Net;

namespace Fab.Web.Exceptions;

public class ExceptionHandler
{
    public (HttpStatusCode Code, object Body) Handle(Exception exception, HttpContext context) =>
        exception switch
        {
            ValidationException validationException => (HttpStatusCode.UnprocessableEntity,
                new ExceptionDto(ExceptionType.Validation,
                    validationException.Errors
                                       .Select(x => new ExceptionDetailDto(x.ErrorCode, x.PropertyName, x.ErrorMessage))
                                       .ToList())),

            RestException { InnerException: AggregateException ae } restException when
                ae.InnerExceptions.All(e => e is RestException) => (restException.Code,
                    new ExceptionDto(ExceptionType.Execution,
                        ae.InnerExceptions
                          .OfType<RestException>()
                          .Select(e => new ExceptionDetailDto(e.Type, e.Message))
                          .ToList())),

            RestException restException => (restException.Code,
                new ExceptionDto(ExceptionType.Execution,
                    new ExceptionDetailDto(restException.Type, restException.Message))),

            NotImplementedException => (HttpStatusCode.NotImplemented,
                new ExceptionDto(ExceptionType.Execution,
                    new ExceptionDetailDto(exception.GetType().Name, @"Метод еще не реализован ¯\_(ツ)_/¯"))),

            _ => (HttpStatusCode.InternalServerError,
                new ExceptionDto(ExceptionType.Execution, exception))
        };

    public bool Report(HttpStatusCode status, Exception exception, HttpRequest request) =>
        status == HttpStatusCode.InternalServerError;
}