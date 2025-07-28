namespace FSP.Domain.Models.DTO
{
    public  class TokenResult
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; } = false;
    }
}