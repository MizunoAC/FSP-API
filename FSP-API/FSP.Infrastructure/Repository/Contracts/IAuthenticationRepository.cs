using FSP.Domain.Enums;
using FSP.Domain.Models;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        Task<MessageResponse> Authentication(UserAuthentication user);
        string TokenGenerationRS(string User, UserType userType);
    }
}
