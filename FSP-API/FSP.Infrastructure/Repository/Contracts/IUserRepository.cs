using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<MessageResponse> RegisterNewUser(UserModelRequest model);
        Task<UserModelDto> GetUserByID(string id);
        Task<MessageResponse> DeleteUser(int UserId);
    }
}