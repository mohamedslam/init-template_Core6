using Fab.UseCases.Handlers.Projects.Commands.CreateProject;
using Fab.UseCases.Handlers.Projects.Commands.DeleteProject;
using Fab.UseCases.Handlers.Projects.Commands.LoadProjectModel;
using Fab.UseCases.Handlers.Projects.Commands.UpdateProject;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Handlers.Projects.Queries.ListProjectDrawings;
using Fab.UseCases.Handlers.Projects.Queries.ListProjectDstvFiles;
using Fab.UseCases.Handlers.Projects.Queries.ListProjectModels;
using Fab.UseCases.Handlers.Projects.Queries.ListProjectModelsMeta;
using Fab.UseCases.Handlers.Projects.Queries.ListProjects;
using Fab.UseCases.Handlers.Projects.Queries.ProjectModelDownload;
using Fab.UseCases.Handlers.Projects.Queries.ReadProject;
using Fab.UseCases.Handlers.Projects.Queries.ReadProjectModel;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Проекты
/// </summary>
[Route("v{version:apiVersion}/projects")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Листинг моделей Project по заданному условию
    /// </summary>
    [HttpGet]
    public Task<Page<ProjectDto>> ProjectsFind([FromQuery] ListProjectsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Создание модели Project
    /// </summary>
    [HttpPost]
    public Task<Guid> ProjectsCreate([FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Чтение модели Project
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}")]
    public Task<ProjectDto> ProjectsRead([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new ReadProjectRequest
        {
            ProjectId = projectId
        }, cancellationToken);
    }

    /// <summary>
    ///     Обновление Project
    /// </summary>
    [HttpPatch]
    [Route("{projectId:guid}")]
    public Task ProjectsUpdate([FromRoute] Guid projectId,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request.Also(x => x.ProjectId = projectId), cancellationToken);
    }

    /// <summary>
    ///     Удаление Project
    /// </summary>
    [HttpDelete]
    [Route("{projectId:guid}")]
    public Task ProjectsRemove([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new DeleteProjectRequest()
        {
            ProjectId = projectId
        }, cancellationToken);
    }
    
    /// <summary>
    ///     Список ресурсов-чертежей в проекте
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/drawings")]
    public Task<Page<ProjectDrawingDto>> ProjectsDrawings([FromQuery] ListProjectDrawingsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Список ресурсов-dstv файлов в проекте
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/dstv-files")]
    public Task<Page<ProjectDstvFilesDto>> ProjectDstvFiles([FromQuery] ListProjectDstvFilesRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Список моделей по проекту
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/models")]
    public Task<Page<ProjectModelDto>> ProjectModels([FromQuery] ListProjectModelsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Загрузка модели для проекта
    /// </summary>
    [HttpPost]
    [Route("{projectId:guid}/models")]
    public Task ProjectModelUpload([FromQuery] LoadProjectModelRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Чтение данные модели проекта в виде набора свойств и значений
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/models/{modelId:guid}")]
    public Task<ProjectModelDto> ProjectModelDataRead([FromQuery] ReadProjectModelRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Получение мета-данных для построения таблицы
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/models/{modelId:guid}/meta")]
    public Task<Page<ProjectModelDto>> ProjectGetMeta([FromQuery] ListProjectModelsMetaRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
    
    /// <summary>
    ///     Скачивание отредактированной модели
    /// </summary>
    [HttpGet]
    [Route("{projectId:guid}/models/{modelId:guid}/download")]
    public Task ProjectsModelDownload([FromQuery] ProjectModelDownloadRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
}