using AutoMapper;
using Fab.Entities.Models.Projects;
using Fab.Infrastructure.DataAccess.Interfaces;
using MediatR;

namespace Fab.UseCases.Handlers.Projects.Commands.CreateProject;

public class CreateProjectRequestHandler  : IRequestHandler<CreateProjectRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateProjectRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = _mapper.Map<Project>(request);

        _dbContext.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}