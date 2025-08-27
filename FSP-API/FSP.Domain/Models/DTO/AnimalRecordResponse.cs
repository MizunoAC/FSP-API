namespace FSP.Domain.Models.DTO
{
    public class AnimalRecordResponse
    {
        public List<AnimalRecordDto> Records { get; set; } = new List<AnimalRecordDto>();
        public PaginationModel? Pagination { get; set; }

    }
}