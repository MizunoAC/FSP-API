namespace FSP.Domain.Models.DTO
{
    public class UsersDtoResponse
    {
        public List<UserModelDto> Users { get; set; } = new List<UserModelDto>();
        public PaginationModel? Pagination { get; set; }
    }
}
