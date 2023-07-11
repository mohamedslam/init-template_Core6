using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Resources.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Resources.Queries.ReadResource;

public class ReadResourceRequestHandler : IRequestHandler<ReadResourceRequest, ResourceDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadResourceRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ResourceDto> Handle(ReadResourceRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Resources
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .ById(request.ResourceId)
                        .ProjectTo<ResourceDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Ресурс не найден");
}