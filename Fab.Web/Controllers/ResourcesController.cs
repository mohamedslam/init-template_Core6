using Fab.UseCases.Handlers.Resources.Commands.DeleteResource;
using Fab.UseCases.Handlers.Resources.Commands.UpdateResource;
using Fab.UseCases.Handlers.Resources.Dto;
using Fab.UseCases.Handlers.Resources.Queries.ListResources;
using Fab.UseCases.Handlers.Resources.Queries.ReadResource;
using Fab.UseCases.Handlers.Resources.Queries.ResolveResource;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using CreateResourceCommand = Fab.UseCases.Handlers.Resources.Commands.CreateResource.CreateResourceRequest;

namespace Fab.Web.Controllers;

/// <summary>
///     Ресурсы
/// </summary>
[Route("v{version:apiVersion}/resources")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class ResourcesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourcesController(IMediator mediator) =>
        _mediator = mediator;

    /// <summary>
    ///     Листинг ресурсов
    /// </summary>
    [HttpGet]
    public Task<Page<ResourceDto>> ListResources([FromQuery] ListResourcesRequest request,
                                                 CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);

    /// <summary>
    ///     Создание ресурса
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public Task<Guid> CreateResource([FromForm] CreateResourceRequest request,
                                     CancellationToken cancellationToken) =>
        _mediator.Send(new CreateResourceCommand
        {
            Name = request.Name,
            File = request.File != null
                ? new()
                {
                    Size = request.File.Length,
                    ContentType = request.File.ContentType,
                    Content = request.File.OpenReadStream(),
                    OriginalName = Path.GetFileName(request.File.FileName),
                    Extension = Path.GetExtension(request.File.FileName)
                }
                : null!
        }, cancellationToken);

    /// <summary>
    ///     Чтение ресурса
    /// </summary>
    [HttpGet]
    [Route("{resourceId:guid}")]
    public Task<ResourceDto> ReadResource([FromRoute] Guid resourceId,
                                          CancellationToken cancellationToken) =>
        _mediator.Send(new ReadResourceRequest
        {
            ResourceId = resourceId
        }, cancellationToken);

    /// <summary>
    ///     Скачивание ресурса
    /// </summary>
    [HttpGet]
    [Route("{resourceId:guid}/resolve")]
    public async Task<IActionResult> ResolveResource([FromRoute] Guid resourceId,
                                                     CancellationToken cancellationToken)
    {
        var uri = await _mediator.Send(new ResolveResourceRequest
        {
            ResourceId = resourceId
        }, cancellationToken);

        return Redirect(uri.AbsoluteUri);
    }

    /// <summary>
    ///     Обновление ресурса
    /// </summary>
    [HttpPatch]
    [Route("{resourceId:guid}")]
    public Task UpdateResource([FromRoute] Guid resourceId,
                               [FromBody] UpdateResourceRequest request,
                               CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(x => x.ResourceId = resourceId), cancellationToken);

    /// <summary>
    ///     Удаление ресурса
    /// </summary>
    [HttpDelete]
    [Route("{resourceId:guid}")]
    public Task DeleteResource([FromRoute] Guid resourceId,
                               CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteResourceRequest
        {
            ResourceId = resourceId
        }, cancellationToken);
}

public class CreateResourceRequest
{
    /// <summary>
    ///     Название ресурса
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Файл
    /// </summary>
    public IFormFile? File { get; set; } = null!;
}