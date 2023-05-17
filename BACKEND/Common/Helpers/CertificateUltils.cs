using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Common.Helpers
{
    public static class CertificateUltils
    {
        public static RSA CreateRsaPublicKey(X509Certificate2 certificate)
        {
            RSA publicKeyProvider = certificate.GetRSAPublicKey();
            return publicKeyProvider;
        }

        public static RSA CreateRsaPrivateKey(X509Certificate2 certificate)
        {
            RSA privateKeyProvider = certificate.GetRSAPrivateKey();
            return privateKeyProvider;
        }
    }
}
