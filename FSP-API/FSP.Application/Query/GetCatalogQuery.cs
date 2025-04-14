using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetCatalogQuery :IRequest<List<CatalogDto>>
    {

    }

    public class GetCatalogQueryHandler : IRequestHandler<GetCatalogQuery, List<CatalogDto>>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetCatalogQueryHandler(IAnimalRepository animalRepository)
        {
           _animalRepository = animalRepository; 
        }

        public async Task<List<CatalogDto>> Handle(GetCatalogQuery request, CancellationToken cancellationToken)
        {
           return await _animalRepository.GetCatalog();
        }
    }
}