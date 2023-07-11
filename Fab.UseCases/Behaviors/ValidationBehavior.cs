using Autofac;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fab.UseCases.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly ILifetimeScope _scope;

    public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        ILifetimeScope scope)
    {
        _logger = logger;
        _scope = scope;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var validator = _scope.ResolveOptional<IValidator<TRequest>>();

        if (validator != null)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Validator for {Type} not found", request.GetType().FullName);
        }

        return await next();
    }
}