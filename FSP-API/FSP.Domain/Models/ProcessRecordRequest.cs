namespace FSP.Domain.Models
{
    public class ProcessRecordRequest
    {
        public int RecordId { get; set; }
        public string Status { get; set; }
        public string RejectedReason { get; set; }
    }
}