using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetCatalogById : IRequest<CatalogDto>
    {
        public int CatalogId { get; set; }
        public GetCatalogById( int catalogId)
        {
            CatalogId = catalogId;
        }
    }

    public class GetCatalogByCommonNounQueryHandler : IRequestHandler<GetCatalogById, CatalogDto>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetCatalogByCommonNounQueryHandler(IAnimalRepository animalRepository)
        {
           _animalRepository = animalRepository; 
        }

        public async Task<CatalogDto> Handle(GetCatalogById request, CancellationToken cancellationToken)
        {
           var result = await _animalRepository.GetCatalogById(request.CatalogId);
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            var imageName = $"{result.Image}.png";
            result.Image = $"{baseUrl}/catalog/{imageName}";
            return result;
        }
    }
}