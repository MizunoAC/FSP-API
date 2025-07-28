using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query 
{
    public class GetCatalogMapQuery : IRequest<CatalogMapDto>
    {
        public int CatalogId { get; set; }
        public GetCatalogMapQuery(int catalogId)   
        { 
            CatalogId = catalogId;
        }
    }

    public class UpdateCatalogMapCommandHandller : IRequestHandler<GetCatalogMapQuery, CatalogMapDto>
    {
        private readonly IAnimalRepository _repository;

        public UpdateCatalogMapCommandHandller(IAnimalRepository Repository)
        {
            _repository = Repository;
        }

        public async Task<CatalogMapDto> Handle(GetCatalogMapQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetCatalogMap(request.CatalogId);
            return result;
        }
    }
}