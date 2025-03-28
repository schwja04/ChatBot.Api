using System.Security.Cryptography.X509Certificates;
using Common.Mongo.Models;
using Microsoft.Extensions.Options;

namespace Common.Mongo;

public class X509Manager : IX509Manager
{
    private X509Certificate2? _certificate;
    private PkiConfigurationRecord _pkiConfigurationRecord;

    public X509Manager(IOptionsMonitor<PkiConfigurationRecord> pkiOptions)
    {
        _pkiConfigurationRecord = pkiOptions.CurrentValue;

        pkiOptions.OnChange(OnCertificateChange);
    }

    public X509Manager(PkiConfigurationRecord pkiConfigurationRecord)
    {
        _pkiConfigurationRecord = pkiConfigurationRecord;

        _certificate = LoadPkiCertificate(pkiConfigurationRecord);
    }

    public X509Certificate2? GetCertificate()
    {
        return _certificate ??= LoadPkiCertificate(_pkiConfigurationRecord);
    }

    private static X509Certificate2? LoadPkiCertificate(PkiConfigurationRecord pkiConfigurationRecord)
    {
        if (string.IsNullOrWhiteSpace(pkiConfigurationRecord.ClientCertificate) ||
            string.IsNullOrWhiteSpace(pkiConfigurationRecord.ClientPrivateKey))
        {
            // throw new InvalidOperationException("Client certificate or private key is missing");
            return null;
        }

        using var certificateFromDisk = X509Certificate2.CreateFromPem(pkiConfigurationRecord.ClientCertificate, pkiConfigurationRecord.ClientPrivateKey);

        return new X509Certificate2(certificateFromDisk.Export(X509ContentType.Pfx));
    }

    private void OnCertificateChange(PkiConfigurationRecord newPkiConfigurationRecord)
    {
        if (_pkiConfigurationRecord.ClientCertificate != newPkiConfigurationRecord.ClientCertificate ||
            _pkiConfigurationRecord.ClientPrivateKey != newPkiConfigurationRecord.ClientPrivateKey)
        {
            _pkiConfigurationRecord = newPkiConfigurationRecord;

            _certificate = LoadPkiCertificate(newPkiConfigurationRecord);
            _pkiConfigurationRecord = newPkiConfigurationRecord;
        }
    }
}
