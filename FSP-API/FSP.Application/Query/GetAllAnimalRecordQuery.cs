using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Sprache;

namespace FSP.Application.Query
{
    public class GetAllAnimalRecordQuery : IRequest<AdminAnimalRecordResponse>
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

    public class GetAllAnimalRecordQueryQueryHandler(IAnimalRepository animalRepository) : IRequestHandler<GetAllAnimalRecordQuery, AdminAnimalRecordResponse>
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;

        public async Task<AdminAnimalRecordResponse> Handle(GetAllAnimalRecordQuery request, CancellationToken cancellationToken)
        {
            var result =  await _animalRepository.GetAllRecords(request.RecordStatus, request.PageNumber, request.PageSize);
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            foreach (var record in result.Records)
            {
                if (string.IsNullOrEmpty(record.img))
                    continue;

                var imageName = $"{record.img}.png";
                record.img = $"{baseUrl}/records/{imageName}";
            }
            return result;
        }
    }
}