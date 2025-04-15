using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetAllAnimalRecordQuery : IRequest<List<AnimalRecordDto>>
    {
        public string RecordStatus { get; set; }
        public GetAllAnimalRecordQuery(string recordStatus)
        {
            RecordStatus = recordStatus;
        }
    }

    public class GetAllAnimalRecordQueryQueryHandler(IAnimalRepository animalRepository) : IRequestHandler<GetAllAnimalRecordQuery, List<AnimalRecordDto>>
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;

        public Task<List<AnimalRecordDto>> Handle(GetAllAnimalRecordQuery request, CancellationToken cancellationToken)
        {
            return  _animalRepository.GetAllRecords(request.RecordStatus);
        }
    }
}