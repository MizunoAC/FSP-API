using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetCatalogCommonNounQuery : IRequest<IList<CatalogCommonNoun>>
    {

    }

    public class GetCatalogCommonNounQueryHandler : IRequestHandler<GetCatalogCommonNounQuery, IList<CatalogCommonNoun>>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetCatalogCommonNounQueryHandler(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public Task<IList<CatalogCommonNoun>> Handle(GetCatalogCommonNounQuery request, CancellationToken cancellationToken)
        {
            return _animalRepository.GetCommounName();
        }
    }
    
}
