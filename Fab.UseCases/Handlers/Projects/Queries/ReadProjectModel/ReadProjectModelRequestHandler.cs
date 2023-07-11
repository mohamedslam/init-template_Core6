using AutoMapper;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Projects.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Projects.Queries.ReadProjectModel;

public class ReadProjectModelRequestHandler : IRequestHandler<ReadProjectModelRequest, ProjectModelDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadProjectModelRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<ProjectModelDto> Handle(ReadProjectModelRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}