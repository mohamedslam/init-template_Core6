using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Users.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Queries.ReadUser;

public class ReadUserRequestHandler : IRequestHandler<ReadUserRequest, UserDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadUserRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(ReadUserRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Users
                        .AsTracking()
                        .AsSplitQuery()
                        .WithScope(request.Scope)
                        .ById(request.UserId)
                        .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Пользователь не найден");
}