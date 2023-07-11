using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Projects.Commands.UpdateProject;

public class UpdateProjectRequestHandler: IRequestHandler<UpdateProjectRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateProjectRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
                           .WithScope(request.Scope)
                           .ById(request.ProjectId)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Проект не найден");

        _mapper.Map(request, project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}