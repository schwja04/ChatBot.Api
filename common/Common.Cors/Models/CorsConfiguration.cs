namespace Common.Cors.Models;

public class CorsConfiguration
{
    public static readonly string RootKey = "Cors";

    public string PolicyName { get; set; } = null!;
    public string[] AllowedOrigins { get; set; } = new string[0];
}
