using AutoMapper;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Authentication.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.RefreshToken;

public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, AuthTokenDto>
{
    private readonly IDbContext _context;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public RefreshTokenRequestHandler(IDbContext context, IAuthenticationService authenticationService, IMapper mapper)
    {
        _context = context;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<AuthTokenDto> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var (auth, user) = await _authenticationService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user.IsBlocked)
        {
            throw new ForbiddenException("Пользователь заблокирован");
        }

        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AuthTokenDto>(auth);
    }
}