using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Projects.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Projects.Queries.ReadProject;

public class ReadProjectRequestHandler : IRequestHandler<ReadProjectRequest, ProjectDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadProjectRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(ReadProjectRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Projects
            .AsNoTracking()
            .WithScope(request.Scope)
            .ById(request.ProjectId)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Проект не найден");
}