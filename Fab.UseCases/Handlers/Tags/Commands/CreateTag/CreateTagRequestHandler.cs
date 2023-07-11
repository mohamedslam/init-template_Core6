using AutoMapper;
using Fab.Entities.Models.Tags;
using Fab.Infrastructure.DataAccess.Interfaces;
using MediatR;

namespace Fab.UseCases.Handlers.Tags.Commands.CreateTag;

public class CreateTagRequestHandler : IRequestHandler<CreateTagRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateTagRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var tag = _mapper.Map<Tag>(request);
        _dbContext.Add(tag);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return tag.Id;
    }
}