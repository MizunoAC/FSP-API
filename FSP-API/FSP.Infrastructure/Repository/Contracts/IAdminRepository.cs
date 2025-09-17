using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAdminRepository
    {

        Task<MessageResponse> ProcessRecord(int recordId, string status, string userId);
        Task<UserEmailData> GetEmailData(int recordId);
        Task<MessageResponse> InsertNewCatalog(CatalogRequest model, string userId);
        Task<MessageResponse> UpdateCatalog(CatalogRequestDto catalog, string userId);
        Task<string> GetCatalogImage(int catalogId);
        Task<UsersDtoResponse> GetAllUsers(int pageNumber, int pageSize);
    }
}
