using Fab.UseCases.Handlers.Tags.Commands.CreateTag;
using Fab.UseCases.Handlers.Tags.Commands.DeleteTag;
using Fab.UseCases.Handlers.Tags.Commands.UpdateTag;
using Fab.UseCases.Handlers.Tags.Dto;
using Fab.UseCases.Handlers.Tags.Queries.ListTags;
using Fab.UseCases.Handlers.Tags.Queries.ReadTag;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Теги
/// </summary>
[Route("v{version:apiVersion}/tags")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Листинг тегов
    /// </summary>
    [HttpGet]
    public Task<Page<TagDto>> ListTags([FromQuery] ListTagsRequest request,
                                       CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Создание тега
    /// </summary>
    [HttpPost]
    public Task<Guid> CreateTag([FromBody] CreateTagRequest request,
                                CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Чтение тега
    /// </summary>
    [HttpGet]
    [Route("{tagId:guid}")]
    public Task<TagDto> ReadTag([FromRoute] Guid tagId,
                                CancellationToken cancellationToken)
    {
        return _mediator.Send(new ReadTagRequest
        {
            TagId = tagId
        }, cancellationToken);
    }

    /// <summary>
    ///     Обновление тега
    /// </summary>
    [HttpPatch]
    [Route("{tagId:guid}")]
    public Task UpdateTag([FromRoute] Guid tagId,
                          [FromBody] UpdateTagRequest request,
                          CancellationToken cancellationToken)
    {
        return _mediator.Send(request.Also(x => x.TagId = tagId), cancellationToken);
    }

    /// <summary>
    ///     Удаление тега
    /// </summary>
    [HttpDelete]
    [Route("{tagId:guid}")]
    public Task DeleteTag([FromRoute] Guid tagId,
                          CancellationToken cancellationToken)
    {
        return _mediator.Send(new DeleteTagRequest
        {
            TagId = tagId
        }, cancellationToken);
    }
}