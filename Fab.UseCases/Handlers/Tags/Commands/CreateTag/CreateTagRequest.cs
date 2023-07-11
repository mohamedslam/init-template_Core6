using MediatR;

namespace Fab.UseCases.Handlers.Tags.Commands.CreateTag;

public class CreateTagRequest : IRequest<Guid>
{
    /// <summary>
    ///     Заголовок тега
    /// </summary>
    public string Label { get; set; } = null!;
}