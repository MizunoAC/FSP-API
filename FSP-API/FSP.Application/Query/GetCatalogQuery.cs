using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

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
           return await _animalRepository.GetCatalog(request.PageNumber, request.PageSize);
        }
    }
}