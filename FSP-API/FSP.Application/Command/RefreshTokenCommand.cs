using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using System.Net;

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
          var result =  authenticationRepository.RefreshToken(request.RefreshToken);

            if (result.Error)
            {
                throw new HttpRequestException(result.Message, null, HttpStatusCode.Unauthorized);
            }
            return result;
        }
    }

}
