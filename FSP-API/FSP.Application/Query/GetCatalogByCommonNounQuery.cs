using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetCatalogByCommonNounQuery : IRequest<CatalogDto>
    {
        public string CommonNoun { get; set; }
        public GetCatalogByCommonNounQuery( string commonNoun)
        {
            CommonNoun = commonNoun;
        }
    }

    public class GetCatalogByCommonNounQueryHandler : IRequestHandler<GetCatalogByCommonNounQuery, CatalogDto>
    {
        private readonly IAnimalRepository _animalRepository;

        public GetCatalogByCommonNounQueryHandler(IAnimalRepository animalRepository)
        {
           _animalRepository = animalRepository; 
        }

        public async Task<CatalogDto> Handle(GetCatalogByCommonNounQuery request, CancellationToken cancellationToken)
        {
           return await _animalRepository.GetCatalogByCommonNoun(request.CommonNoun);
        }
    }
}