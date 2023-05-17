using CertificateManager;
using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Common.Helpers
{
    public  class CreateRsaCertificates
    {
        public static X509Certificate2 CreateRsaCertificateSimple()
        {
            using var rsa = RSA.Create();
            var certRequest = new CertificateRequest("cn=DTC", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(800));
            return certificate;
        }
      
        public static X509Certificate2 CreateRsaCertificate(CreateCertificates createCertificates, int keySize)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 2,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    "SigningCertificateTest",
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign
               | X509KeyUsageFlags.DigitalSignature
               | X509KeyUsageFlags.KeyEncipherment
               | X509KeyUsageFlags.CrlSign
               | X509KeyUsageFlags.DataEncipherment
               | X509KeyUsageFlags.NonRepudiation
               | X509KeyUsageFlags.KeyAgreement;

            // only if mtls is used
            var enhancedKeyUsages = new OidCollection
            {
                //OidLookup.ClientAuthentication,
                //OidLookup.ServerAuthentication,
                OidLookup.CodeSigning,
                OidLookup.SecureEmail,
                OidLookup.TimeStamping
            };

            var certificate = createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = "SigningCertificateTest" },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow.AddDays(-100),
                    ValidTo = DateTimeOffset.UtcNow.AddYears(1)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                {
                    KeySize = keySize,
                    RSASignaturePadding = RSASignaturePadding.Pkcs1,
                    HashAlgorithmName = HashAlgorithmName.SHA256
                });

            return certificate;
        }
    }
}
