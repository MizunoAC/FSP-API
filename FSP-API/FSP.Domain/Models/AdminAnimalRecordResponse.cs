namespace FSP.Domain.Models.DTO
{
    public class AdminAnimalRecordResponse
    {
        public List<AdminAnimalRecord> Records { get; set; } = new List<AdminAnimalRecord>();
        public PaginationModel? Pagination { get; set; }

    }
}