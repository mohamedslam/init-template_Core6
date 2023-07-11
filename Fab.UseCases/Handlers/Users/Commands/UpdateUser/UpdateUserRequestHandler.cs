using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.UpdateUser;

public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateUserRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .WithScope(request.Scope)
                                   .ById(request.UserId)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        _mapper.Map(request, user);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}