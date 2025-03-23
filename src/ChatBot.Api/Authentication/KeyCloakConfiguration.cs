using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Authentication;

public class KeycloakConfiguration
{
    public KeycloakConfiguration(IConfiguration configuration)
    {
        var keyCloakSection = configuration.GetSection("Authorization:Keycloak");
        if (!keyCloakSection.Exists())
        {
            throw new ArgumentException(
                "No child configuration section found in 'Authorization' for Keycloak.",
                nameof(configuration));
        }

        var baseAddress = keyCloakSection.GetValue<Uri>("BaseAddress");
        if (baseAddress is not null && !baseAddress.AbsoluteUri.EndsWith('/'))
        {
            baseAddress = new Uri($"{baseAddress.AbsoluteUri}/");
        }

        BaseAddress = baseAddress!;

        Realm = keyCloakSection.GetValue<string>("Realm")!;

        Audience = keyCloakSection.GetValue<string>("Audience")!;

        Authority = new Uri($"{BaseAddress}/auth/realms/{Realm}/protocol/openid-connect/auth");
    }

    public string Audience { get; }
    public Uri Authority { get; }
    public Uri BaseAddress { get; }
    public string Realm { get; }
}

public record KeycloakConfigurationRecord
{
    private Uri _baseAddress;

    [Required]
    [MinLength(1)]
    public required string Audience { get; init; }
    
    [Required]
    public Uri BaseAddress
    {
        get => _baseAddress;
        set
        {
            if (!value.AbsoluteUri.EndsWith('/'))
            {
                _baseAddress = new Uri($"{value.AbsoluteUri}/");
                return;
            }
            _baseAddress = value;
        }
    }

    [Required]
    [MinLength(1)]
    public required string Realm { get; init; }
    
    public Uri Authority => new Uri($"{BaseAddress}/auth/realms/{Realm}/protocol/openid-connect/auth", UriKind.Absolute);
}