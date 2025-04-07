using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<MessageResponse> RegisterNewUser(UserModelRequest model);
        Task<UserModelDto> GetUserByID(int id);
        Task<MessageResponse> DeleteUser(int UserId);
    }
}