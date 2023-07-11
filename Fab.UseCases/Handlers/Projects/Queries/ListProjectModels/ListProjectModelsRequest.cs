using Fab.Entities.Abstractions;
using Fab.Entities.Models.Projects;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Pagination;
using Fab.UseCases.Support.Scopes;
using Fab.UseCases.Support.Search;
using Fab.UseCases.Support.Sorts;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Projects.Queries.ListProjectModels;

public class ListProjectModelsRequest : IRequest<Page<ProjectModelDto>>, IScopedRequest<ProjectModel>,
    IPaginationRequest, IMultipleSortingRequest, IFilterRequest, ISearchRequest
{
    [NotMapped]
    public Spec<ProjectModel>? Scope { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Page"/>
    public int Page { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Limit"/>
    public int? Limit { get; set; }

    /// <inheritdoc cref="IMultipleSortingRequest.Sorts"/>
    public ICollection<Sorting>? Sorts { get; set; }

    /// <inheritdoc cref="IFilterRequest.Filter"/>
    public string? Filter { get; set; }

    /// <inheritdoc cref="ISearchRequest.Query"/>
    public string? Query { get; set; }
}