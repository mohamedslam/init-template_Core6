using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Resources.Commands.UpdateResource;

public class UpdateResourceRequestHandler : IRequestHandler<UpdateResourceRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateResourceRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateResourceRequest request, CancellationToken cancellationToken)
    {
        var resource = await _dbContext.Resources
                                       .WithoutTrashed()
                                       .WithScope(request.Scope)
                                       .ById(request.ResourceId)
                                       .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Ресурс не найден");

        _mapper.Map(request, resource);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}