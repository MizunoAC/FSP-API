using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Sprache;

namespace FSP.Application.Query
{
    public class GetCatalogQuery :IRequest<CatalogResponse>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public GetCatalogQuery(int pageNumber, int pageSize)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

    }

    public class GetCatalogQueryHandler : IRequestHandler<GetCatalogQuery, CatalogResponse>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetCatalogQueryHandler(IAnimalRepository animalRepository)
        {
           _animalRepository = animalRepository; 
        }

        public async Task<CatalogResponse> Handle(GetCatalogQuery request, CancellationToken cancellationToken)
        {
           var result = await _animalRepository.GetCatalog(request.PageNumber, request.PageSize);

            foreach (var catalog in result.Catalog)
            {
                if (string.IsNullOrEmpty(catalog.Image))
                    continue;
                var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
                var imageName = $"{catalog.Image}.png";
                catalog.Image = $"{baseUrl}/catalog/{imageName}";
            }
            return result;
        }
    }
}