using Fab.Web.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

namespace Fab.Web.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void AddApiExceptionHandler(this IServiceCollection services) =>
        services.PostConfigure<ApiBehaviorOptions>(options =>
            options.InvalidModelStateResponseFactory = ctx =>
                new UnprocessableEntityObjectResult(
                    new ExceptionDto(ExceptionType.Validation,
                        ctx.ModelState
                           .Where(x => x.Value != null)
                           .SelectMany(x => x.Value!
                                             .Errors
                                             .Select(e => new ExceptionDetailDto(
                                                 x.Value!.ValidationState.ToString(),
                                                 x.Key,
                                                 e.ErrorMessage)))
                           .ToList())));

    public static void UseApiExceptionHandler(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<ExceptionHandler>>();
        var handler = new ExceptionHandler();

        app.UseExceptionHandler(pipeline =>
        {
            pipeline.Run(context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                if (feature == null)
                {
                    return Task.CompletedTask;
                }

                context.Response.ContentType = MediaTypeNames.Application.Json;

                try
                {
                    var (code, response) = handler.Handle(feature.Error, context);

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    context.Response.StatusCode = (int)code;
                    return context.Response.WriteAsJsonAsync(response);
                }
                catch (Exception e)
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    return context.Response.WriteAsJsonAsync(new ExceptionDto(ExceptionType.Execution,
                        new ExceptionDetailDto(e.GetType().Name,
                            !string.IsNullOrEmpty(e.Message)
                                ? e.Message
                                : "Что-то пошло не так")));
                }
                finally
                {
                    if (handler.Report(Enum.Parse<HttpStatusCode>(context.Response.StatusCode.ToString()),
                            feature.Error,
                            context.Request))
                    {
                        logger.LogError(feature.Error,
                            "An unhandled exception has occurred while executing the request.");
                    }
                }
            });
        });
    }
}