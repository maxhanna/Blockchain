using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Blockchain.Core
{
    public class Block
    {
        public int Index { get; set; }
        public long Timestamp { get; set; }
        public List<string> Transactions { get; set; }
        public string PreviousHash { get; set; }
        public int Nonce { get; set; }
        public string Hash => Pow.ComputeBlockHash(this);

        private string ComputeHash()
        {
            var header = $"{Index}{Timestamp}{JsonConvert.SerializeObject(Transactions)}{PreviousHash}{Nonce}";
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(header));
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
    }

    public class Blockchain
    {
        private readonly int _difficulty;
        private readonly FileStorage _storage;
        public List<Block> Chain { get; private set; }
        public List<string>? Peers { get; set; }
        public List<string> CurrentTransactions { get; } = new();

        public Blockchain(int difficulty = 4, FileStorage storage = null)
        {
            _difficulty = difficulty;
            _storage = storage ?? new FileStorage("chain.json");
            Chain = _storage.Load();
            if (!Chain.Any()) CreateGenesisBlock();
        }

        private void CreateGenesisBlock()
        {
            var genesis = new Block
            {
                Index = 0,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Transactions = new List<string>(),
                PreviousHash = "0",
                Nonce = 0
            };
            Chain.Add(genesis);
            _storage.Save(Chain);
        }
        public void ConnectPeer(string peerUrl)
        {
            if (Peers == null)
            {
                Peers = new List<String>();
            }
            if (!Peers.Contains(peerUrl))
                Peers.Add(peerUrl);
        }
        public void MineGenesisBlock()
        {
            var genesis = new Block
            {
                Index = 0,
                Timestamp = 1710000000,
                Transactions = new List<string>(),
                PreviousHash = "0",
                Nonce = Pow.ComputeProof("0", _difficulty)
            };

            Chain.Add(genesis);
            _storage.Save(Chain);
        }
        public void AddTransaction(string tx) => CurrentTransactions.Add(tx);
        public decimal GetBalance(string address)
        {
            decimal balance = 0;
            foreach (var block in Chain)
            {
                foreach (var txJson in block.Transactions)
                {
                    var tx = JsonConvert.DeserializeObject<Transaction>(txJson)!;
                    if (tx.ToAddress == address) balance += tx.Amount;
                    if (tx.FromAddress == address) balance -= tx.Amount;
                }
            }
            return balance;
        }
        public Block LastBlock() => Chain.Last();

        public Block MineBlock()
        {
            var lastHash = LastBlock().Hash;
            var nonce = Pow.ComputeProof(lastHash, _difficulty);
            var block = new Block
            {
                Index = Chain.Count,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Transactions = new List<string>(CurrentTransactions),
                PreviousHash = lastHash,
                Nonce = nonce
            };
            Chain.Add(block);
            CurrentTransactions.Clear();
            _storage.Save(Chain);
            return block;
        }

        public bool ValidChain(IEnumerable<Block> chain)
        {
            var blocks = chain.ToList();
            for (int i = 1; i < blocks.Count; i++)
            {
                var prev = blocks[i - 1];
                var curr = blocks[i];
                if (curr.PreviousHash != prev.Hash) return false;
                if (!Pow.ValidateProof(prev.Hash, curr.Nonce, _difficulty)) return false;
            }
            return true;
        }
        /// <summary>
        /// Adds a block to the chain and immediately saves to disk.
        /// </summary>
        public void AddBlock(Block block)
        {
            Chain.Add(block);
            _storage.Save(Chain);    // persist the new chain.json
        }
        /// <summary>
        /// Replaces the current chain if the incoming chain is valid and longer.
        /// </summary>
        public bool ReplaceChain(List<Block> newChain)
        {
            // basic validation: newChain is longer and valid
            Console.WriteLine("Validating chain replacement " + newChain.Count());
            if (newChain.Count <= Chain.Count || !ValidChain(newChain))
                return false;

            Chain = newChain;
            return true;
        }
    }
}