namespace FSP.Domain.Models.DTO
{
    public class CatalogResponse
    {
        public List<CatalogDto> Catalog { get; set; } = new List<CatalogDto>();
        public PaginationModel Pagination { get; set; } = new PaginationModel();
    }
}