namespace FSP.Domain.Models.DTO
{

    public class CatalogMapDto
    {
        public int CatalogId { get; set; }
       public List<CatalogMapCords> Cords { get; set; } = new List<CatalogMapCords>();
    }
    public class CatalogMapCords
    {
        public string Location { get; set; }

    }
}
