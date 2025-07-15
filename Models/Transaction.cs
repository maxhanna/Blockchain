using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Yarn
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Signature { get; set; }

        [JsonConstructor]
        public Transaction(string sender, string receiver, decimal amount, DateTime timestamp, string signature)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Timestamp = timestamp;
            Signature = signature;
        }

        public Transaction(string sender, string receiver, decimal amount)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Timestamp = DateTime.UtcNow;
            Signature = string.Empty;
        }

        public string ComputeHash()
        {
            using var sha256 = SHA256.Create();
            string raw = $"{Sender}{Receiver}{Amount}{Timestamp:o}";
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes);
        }

        public void Sign(string privateKeyPem)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(privateKeyPem);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{Sender}{Receiver}{Amount}{Timestamp:o}"));
            var sig = ecdsa.SignHash(hash);
            Signature = Convert.ToHexString(sig);
        }

        public bool IsValid()
        {
            if (Sender == "SYSTEM") return true;
            if (string.IsNullOrEmpty(Signature)) return false;
            using var ecdsa = ECDsa.Create();
            // Load public key PEM from local wallet directory by address
            var pubPem = Wallet.GetPublicKeyPem(Sender);
            ecdsa.ImportFromPem(pubPem);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{Sender}{Receiver}{Amount}{Timestamp:o}"));
            return ecdsa.VerifyHash(hash, Convert.FromHexString(Signature));
        }
    }

}