using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Yarn
{
    public class Wallet
    {
        private const string KeyFile = "wallet.pem";
        public string PrivatePem { get; private set; }
        public string PublicPem { get; private set; }

        public Wallet()
        {
            if (File.Exists(KeyFile))
            {
                PrivatePem = File.ReadAllText(KeyFile);
            }
            else
            {
                using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                PrivatePem = ecdsa.ExportPkcs8PrivateKeyPem();
                PublicPem = ecdsa.ExportSubjectPublicKeyInfoPem();
                File.WriteAllText(KeyFile, PrivatePem);
            }
            using var ecdsaLoad = ECDsa.Create();
            ecdsaLoad.ImportFromPem(PrivatePem.ToCharArray());
            PublicPem = ecdsaLoad.ExportSubjectPublicKeyInfoPem();
        }

        public string Address => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(PublicPem))).Substring(0, 40);

        public static string GetPublicKeyPem(string address)
        {
            // In production, map address to stored public key PEM
            // Here we assume local wallet
            return File.ReadAllText(KeyFile);
        }
    }

}
