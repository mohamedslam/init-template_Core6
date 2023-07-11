using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Resources;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Resources.Queries.ResolveResource;

public class ResolveResourceRequestHandler : IRequestHandler<ResolveResourceRequest, Uri>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IResourceService _resourceService;

    public ResolveResourceRequestHandler(IReadonlyDbContext dbContext, IResourceService resourceService)
    {
        _dbContext = dbContext;
        _resourceService = resourceService;
    }

    public async Task<Uri> Handle(ResolveResourceRequest request, CancellationToken cancellationToken)
    {
        var resource = await _dbContext.Resources
                                       .AsNoTracking()
                                       .WithScope(request.Scope)
                                       .ById(request.ResourceId)
                                       .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Ресурс не найден");

        return await _resourceService.GetSignedUrlAsync(resource, cancellationToken);
    }
}