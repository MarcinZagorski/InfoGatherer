namespace InfoGatherer.api.Configuration
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string TokenIssuer { get; set; }
        public int TokenValidityPeriodInSeconds { get; set; }
        public int RefreshTokenValidityPeriodInSeconds { get; set; }
    }
}
