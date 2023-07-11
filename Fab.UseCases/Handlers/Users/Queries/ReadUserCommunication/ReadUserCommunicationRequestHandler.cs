using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Dto;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Queries.ReadUserCommunication;

public class ReadUserCommunicationRequestHandler : IRequestHandler<ReadUserCommunicationRequest, CommunicationDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadUserCommunicationRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CommunicationDto> Handle(ReadUserCommunicationRequest request,
                                               CancellationToken cancellationToken) =>
        await _dbContext.Users
                        .AsNoTracking()
                        .ById(request.UserId)
                        .SelectMany(x => x.Communications)
                        .WithScope(request.Scope)
                        .ById(request.CommunicationId)
                        .ProjectTo<CommunicationDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Клммуникация не найдена");
}