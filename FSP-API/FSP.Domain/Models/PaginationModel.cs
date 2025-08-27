namespace FSP.Domain.Models
{
    public class PaginationModel
    {
            public int Page { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
            public int TotalPages { get; set; }
            public bool HasNext { get; set; }
            public bool HasPrev { get; set; }
    }
}
