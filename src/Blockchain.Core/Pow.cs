using System.Security.Cryptography;
using System.Text; 
using Newtonsoft.Json;
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
        public static bool ValidateProof(string previousHash, int nonce, int difficulty)
        {
            string guess = previousHash + nonce;
            string result = ComputeSha256Hash(guess);
            return result.StartsWith(new string('0', difficulty));
        }
        public static string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2")); // lowercase hex

            return sb.ToString();
        }

        public static string ComputeBlockHash(Block block)
        {
            var header = $"{block.Index}{block.Timestamp}{JsonConvert.SerializeObject(block.Transactions)}{block.PreviousHash}{block.Nonce}";
            return ComputeSha256Hash(header);
        }
    }
}