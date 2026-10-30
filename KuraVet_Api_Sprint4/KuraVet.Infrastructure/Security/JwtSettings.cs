namespace KuraVet.Infrastructure.Security;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public const int TamanhoMinimoChave = 32;

    public string Issuer { get; set; } = "KuraVet.Api";

    public string Audience { get; set; } = "KuraVet.Clients";

    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
