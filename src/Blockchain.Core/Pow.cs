using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Core
{
    public static class Pow
    {
        public static int ComputeProof(string lastHash, int difficulty)
        {
            var target = new string('0', difficulty);
            int nonce = 0;
            using var sha = SHA256.Create();
            while (true)
            {
                var data = Encoding.UTF8.GetBytes(lastHash + nonce);
                var hashBytes = sha.ComputeHash(data);
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                if (hash.StartsWith(target)) return nonce;
                nonce++;
            }
        }

        public static bool ValidateProof(string lastHash, int nonce, int difficulty)
        {
            using var sha = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(lastHash + nonce);
            var hash = BitConverter.ToString(sha.ComputeHash(data)).Replace("-", string.Empty);
            return hash.StartsWith(new string('0', difficulty));
        }
    }
}