using FSP.Domain.Models;
using FSP.Infrastructure.Helpers;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GenerateBibliographicRecordQuery : IRequest<byte[]>
    {
        public int CatalogId { get; set; }

        public GenerateBibliographicRecordQuery(int catalogId)
        {
            CatalogId = catalogId;
        }
    }

    public class GenerateBibliographicRecordQueryHandler : IRequestHandler<GenerateBibliographicRecordQuery, byte[]>
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IPdfGeneratorHelper _pdfGeneratorHelper;

        public GenerateBibliographicRecordQueryHandler(IAnimalRepository animalRepository, IPdfGeneratorHelper pdfGeneratorHelper)
        {
            _animalRepository = animalRepository;
            _pdfGeneratorHelper = pdfGeneratorHelper;
        }

        public async Task<byte[]> Handle(GenerateBibliographicRecordQuery request, CancellationToken cancellationToken)
        {
            var catalog = await _animalRepository.GetCatalogById(request.CatalogId);
            if (catalog == null)
                return Array.Empty<byte>();

            var map = await _animalRepository.GetCatalogMap(request.CatalogId);
            var model = new Catalogbibliographic
            {
                Catalog = catalog,
                Coordinates = map?.Cords
            };

            return await _pdfGeneratorHelper.GenerateBibliographicPdfAsync(model, cancellationToken);
        }
    }
}