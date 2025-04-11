using FSP.Domain.Models;

namespace FSP.Infrastructure.Repository.Contracts
{
    public interface IAnimalRepository
    {
        Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId);
    }
}