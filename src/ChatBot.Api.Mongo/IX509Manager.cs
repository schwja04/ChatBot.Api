using System.Security.Cryptography.X509Certificates;

namespace ChatBot.Api.Mongo;

public interface IX509Manager
{
    X509Certificate2? GetCertificate();
}
