using FSP.Domain.Models;

namespace FSP.Infrastructure.Helpers
{
    public interface IPdfGeneratorHelper
    {
        Task<byte[]> GenerateBibliographicPdfAsync(Catalogbibliographic model, CancellationToken cancellationToken = default);
    }
}
