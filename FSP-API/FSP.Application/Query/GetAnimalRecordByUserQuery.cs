using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetAnimalRecordByUserQuery : IRequest<List<AnimalRecordDto>>
    {
        public string UserId { get; set; }
        public string RecordStatus { get; set; }
        public GetAnimalRecordByUserQuery(string userId, string recordStatus)
        {
            UserId = userId;
            RecordStatus = recordStatus;

        }
    }

    public class GetAnimalRecordByUserQueryHandler : IRequestHandler<GetAnimalRecordByUserQuery, List<AnimalRecordDto>>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetAnimalRecordByUserQueryHandler(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public Task<List<AnimalRecordDto>> Handle(GetAnimalRecordByUserQuery request, CancellationToken cancellationToken)
        {
            return _animalRepository.GetRecordsByUserId(request.UserId, request.RecordStatus);
        }
    }
}