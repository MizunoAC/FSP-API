namespace FSP.Domain.Models.DTO;

public class AdminAnimalRecord
{
   public string UserName { get; set; }
    public int RecordId { get; set; }
    public string CommonNoun { get; set; }
    public string AnimalState { get; set; }
    public string? Description { get; set; }
    public string img { get; set; }
    public string Location { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public string RejectedReason { get; set; }
}