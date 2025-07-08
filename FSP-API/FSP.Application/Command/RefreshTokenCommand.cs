using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Command
{
    public class RefreshTokenCommand : IRequest<TokenResult>
    {
        public string RefreshToken { get; set; }
        public RefreshTokenCommand(RefreshTokenDto refresToken)
        {
            RefreshToken = refresToken.refreshToken;
        }

    }

    public class RefreshTokenCommandHandler(IAuthenticationRepository authenticationRepository) :IRequestHandler<RefreshTokenCommand, TokenResult>
    {
        public async Task<TokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
          return  authenticationRepository.RefreshToken(request.RefreshToken);
        }
    }

}
