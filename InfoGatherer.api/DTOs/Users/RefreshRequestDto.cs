namespace InfoGatherer.api.DTOs.Users
{
    public class RefreshRequestDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
