using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tags.Commands.UpdateTag;

public class UpdateTagRequestHandler : IRequestHandler<UpdateTagRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateTagRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var address = await _dbContext.Tags
                                      .WithScope(request.Scope)
                                      .ById(request.TagId)
                                      .FirstOrDefaultAsync(cancellationToken)
                      ?? throw new NotFoundException("Тег не найден");

        _mapper.Map(request, address);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}