namespace InfoGatherer.api.DTOs.Users
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public dynamic User { get; set; }
    }
}
