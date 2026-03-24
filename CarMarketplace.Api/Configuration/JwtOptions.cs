namespace CarMarketplace.Api.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CarMarketplace";

    public string Audience { get; set; } = "CarMarketplaceUsers";

    public string SecretKey { get; set; } = "ChangeThisToASecretKeyWithAtLeast32Chars!";

    public int ExpiryMinutes { get; set; } = 60;
}
