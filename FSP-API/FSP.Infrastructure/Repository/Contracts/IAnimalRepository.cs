using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAnimalRepository
    {
        Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId);
        Task<AnimalRecordResponse> GetRecordsByUserId(string userId, string recordStatus, int pageNumber, int pageSize);
        Task<CatalogResponse> GetCatalog(int pageNumber, int pageSize);
        Task<CatalogDto> GetCatalogById(int catalogId);
        Task<AdminAnimalRecordResponse> GetAllRecords(string recordStatus, int pageNumber, int pageSize);
        Task<CatalogMapDto> GetCatalogMap(int catalogId);
        Task<IList<CatalogCommonNoun>> GetCommounName();
    }
}