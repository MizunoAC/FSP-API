namespace FSP.Domain.Models.DTO
{
    public class AnimalRecordDto
    {
        public int RecordId { get; set; }
        public string CommonNoun { get; set; }
        public string AnimalState { get; set; }
        public string? Description { get; set; }
        public byte[] img { get; set; } = new byte[0];
        public string Location { get; set; }
    }
}