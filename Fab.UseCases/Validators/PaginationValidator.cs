using Fab.UseCases.Support.Pagination;
using FluentValidation;

namespace Fab.UseCases.Validators;

public class PaginationValidator<T> : AbstractValidator<T>
    where T : IPaginationRequest
{
    public PaginationValidator()
    {
        RuleFor(x => x.Limit).GreaterThanOrEqualTo(0)
                             .When(x => x.Limit.HasValue)
                             .WithMessage("Limit не может быть отрицательным числом");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(0)
                            .WithMessage("Page не может быть отрицательным числом");
    }
}