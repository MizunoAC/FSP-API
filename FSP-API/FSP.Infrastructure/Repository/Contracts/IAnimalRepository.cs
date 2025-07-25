using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAnimalRepository
    {
        Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId);
        Task<List<AnimalRecordDto>> GetRecordsByUserId(string userId, string recordStatus, int pageNumber, int pageSize);
        Task<MessageResponse> InsertNewCatalog(CatalogRequest model);
        Task<List<CatalogDto>> GetCatalog(int pageNumber, int pageSize);
        Task<CatalogDto> GetCatalogByCommonNoun(string commonNoun);
        Task<List<AnimalRecordDto>> GetAllRecords(string recordStatus, int pageNumber, int pageSize);
        Task<MessageResponse> ProcessRecord(int recordId, string status);
        Task<UserEmailData> GetEmailData(int recordId);
        void SendEmailNotificacion(UserEmailData data, string rootemv);
    }
}