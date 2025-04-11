using FSP.Domain.Models;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        Task<string> Authentication(UserAuthentication user);
        string TokenGenerationRS(string User);
    }
}
