using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetAllAnimalRecordQuery : IRequest<AnimalRecordResponse>
    {
        public string RecordStatus { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; }

        public GetAllAnimalRecordQuery(string recordStatus, int pageNumber , int pageSize )
        {
            RecordStatus = recordStatus;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllAnimalRecordQueryQueryHandler(IAnimalRepository animalRepository) : IRequestHandler<GetAllAnimalRecordQuery, AnimalRecordResponse>
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;

        public Task<AnimalRecordResponse> Handle(GetAllAnimalRecordQuery request, CancellationToken cancellationToken)
        {
            return  _animalRepository.GetAllRecords(request.RecordStatus, request.PageNumber, request.PageSize);
        }
    }
}