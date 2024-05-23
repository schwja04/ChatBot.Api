namespace ChatBot.Api.Mongo.Models;

public class PkiConfigurationRecord
{
    public static readonly string RootKey = "Pki";

    public string? ClientCertificate { get; set; }

    public string? ClientPrivateKey { get; set; }
}
