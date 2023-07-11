using AutoMapper;
using Fab.ApplicationServices.Interfaces;
using Fab.Entities.Models.Resources;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Resources;
using Fab.Utils.Extensions;
using MediatR;

namespace Fab.UseCases.Handlers.Resources.Commands.CreateResource;

public class CreateResourceRequestHandler : IRequestHandler<CreateResourceRequest, Guid>
{
    private readonly IContext _context;
    private readonly IDbContext _dbContext;
    private readonly IResourceService _service;
    private readonly IMapper _mapper;

    public CreateResourceRequestHandler(IContext context, IDbContext dbContext, IResourceService service,
                                        IMapper mapper)
    {
        _context = context;
        _dbContext = dbContext;
        _service = service;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateResourceRequest request, CancellationToken cancellationToken)
    {
        var digest = await _service.ComputeResourceHashAsync(request.File.Content, cancellationToken);

        var resource = _mapper.Map<Resource>(request, opt =>
        {
            opt.Items[nameof(IContext)] = _context;
            opt.Items[nameof(Resource.Target)] = Path.Combine("uploads", digest.ToHexString());
        });

        _dbContext.Add(resource);

        await _service.PutResourceAsync(resource, request.File.Content, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return resource.Id;
    }
}