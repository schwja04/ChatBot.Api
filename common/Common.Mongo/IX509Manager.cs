using System.Security.Cryptography.X509Certificates;

namespace Common.Mongo;

public interface IX509Manager
{
    X509Certificate2? GetCertificate();
}
