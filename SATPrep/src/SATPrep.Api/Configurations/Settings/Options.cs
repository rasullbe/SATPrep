namespace SATPrep.Api.Configurations.Settings;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
    public string CookieName { get; set; } = "satprep_access";
    public string RefreshCookieName { get; set; } = "satprep_refresh";
    public string CsrfCookieName { get; set; } = "satprep_csrf";
    public string LoggedInCookieName { get; set; } = "satprep_logged_in";
}

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] Origins { get; set; } = Array.Empty<string>();
}

public class AppOptions
{
    public const string SectionName = "App";

    public string FrontendUrl { get; set; } = string.Empty;
    public int FrontendPort { get; set; } = 3000;
}