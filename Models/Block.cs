using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Yarn
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string PreviousHash { get; set; }
        public int Nonce { get; set; }
        public string Hash { get; set; }

        [JsonConstructor]
        public Block(int index, DateTime timestamp, List<Transaction> transactions, string previousHash, int nonce, string hash)
        {
            Index = index;
            Timestamp = timestamp;
            Transactions = transactions;
            PreviousHash = previousHash;
            Nonce = nonce;
            Hash = hash;
        }

        public Block(int index, List<Transaction> transactions, string previousHash)
        {
            Index = index;
            Timestamp = DateTime.UtcNow;
            Transactions = transactions;
            PreviousHash = previousHash;
            Nonce = 0;
            Hash = ComputeHash();
        }

        public string ComputeHash()
        {
            using var sha256 = SHA256.Create();
            var raw = new StringBuilder();
            raw.Append(Index).Append(Timestamp.ToString("o")).Append(PreviousHash).Append(Nonce);
            foreach (var tx in Transactions) raw.Append(tx.ComputeHash());
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw.ToString()));
            return Convert.ToHexString(bytes);
        }

        public void Mine(int difficulty)
        {
            var target = new string('0', difficulty);
            while (!Hash.StartsWith(target, StringComparison.Ordinal))
            {
                Nonce++;
                Hash = ComputeHash();
            }
            Console.WriteLine($"[Block {Index}] Mined: {Hash}");
        }
    }

}
