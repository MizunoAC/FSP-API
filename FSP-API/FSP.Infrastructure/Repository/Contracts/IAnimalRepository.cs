using FSP.Domain.Models;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAnimalRepository
    {
        Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId);
        Task<List<AnimalRecordDto>> GetRecordsByUserId(string userId, string recordStatus);
        Task<MessageResponse> InsertNewCatalog(CatalogRequest model);
        Task<List<CatalogDto>> GetCatalog();
        Task<CatalogDto> GetCatalogByCommonNoun(string commonNoun);
        Task<List<AnimalRecordDto>> GetAllRecords(string recordStatus);
    }
}