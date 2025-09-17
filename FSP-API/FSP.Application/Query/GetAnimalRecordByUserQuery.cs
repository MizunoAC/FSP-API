using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using static System.Net.Mime.MediaTypeNames;

namespace FSP.Application.Query
{
    public class GetAnimalRecordByUserQuery : IRequest<AnimalRecordResponse>
    {
        public string UserId { get; set; }
        public string RecordStatus { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAnimalRecordByUserQuery(string userId, string recordStatus, int pageNumber, int pageSize)
        {
            UserId = userId;
            RecordStatus = recordStatus;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }

    public class GetAnimalRecordByUserQueryHandler : IRequestHandler<GetAnimalRecordByUserQuery, AnimalRecordResponse>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetAnimalRecordByUserQueryHandler(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<AnimalRecordResponse> Handle(GetAnimalRecordByUserQuery request, CancellationToken cancellationToken)
        {
            var result = await _animalRepository.GetRecordsByUserId(request.UserId, request.RecordStatus, request.PageNumber, request.PageSize);
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            foreach (var record in result.Records)
            {
                if (string.IsNullOrEmpty(record.img))
                    continue;

                var imageName = $"{record.img}.png";

                record.img = $"{baseUrl}/records/{record.img}.png";
            }
           
            return result;
        }
    }
}