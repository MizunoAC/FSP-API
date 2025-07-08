using FSP.Domain.Enums;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        Task<TokenResult> Authentication(UserAuthentication user);
        TokenResult TokenGenerationRS(string User, UserType userType);
        Task<string> GenerateResetCode(string email);
        Task<string> ResetPassword(ResetPasswordDTO reset);
        Task<MessageResponse> VerifyCode(string email, int code);
        TokenResult RefreshToken(string refreshToken);
    }
}