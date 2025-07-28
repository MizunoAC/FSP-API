using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAdminRepository
    {

        Task<MessageResponse> ProcessRecord(int recordId, string status);
        Task<UserEmailData> GetEmailData(int recordId);
        void SendEmailNotificacion(UserEmailData data, string rootemv);
        Task<MessageResponse> InsertNewCatalog(CatalogRequest model);
        Task<MessageResponse> UpdateCatalog(CatalogRequestDto catalog);
        Task<MessageResponse> UpdateCatalogImg(CatalogImgDto catalogImg);
    }
}
