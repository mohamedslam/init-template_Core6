using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Tags.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tags.Queries.ReadTag;

public class ReadTagRequestHandler : IRequestHandler<ReadTagRequest, TagDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadTagRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TagDto> Handle(ReadTagRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Tags
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .ById(request.TagId)
                        .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Тег не найден");
}