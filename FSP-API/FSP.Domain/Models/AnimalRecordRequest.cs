namespace FSP.Domain.Models
{
    public class AnimalRecordRequest
    {
        public int CatalogId { get; set; }
        public string CommonNoun { get; set; }
        public int AnimalState { get; set; } 
        public string? Description {  get; set; }
        public string img { get; set; }
        public string Location { get; set; }
    }
}