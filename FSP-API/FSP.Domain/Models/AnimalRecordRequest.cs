namespace FSP.Domain.Models
{
    public class AnimalRecordRequest
    {
        //public string UserId { get; set; }
        public string CommonNoun { get; set; }
        public int AnimalState { get; set; } 
        public string? Description {  get; set; }
        public byte[] img { get; set; } = new byte[0];
        public string Location { get; set; }
    }
}