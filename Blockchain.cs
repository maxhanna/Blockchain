using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Yarn
{
    public class Blockchain
    {
        private const string FileName = "chain.json";
        public List<Block> Chain { get; private set; }
        public List<Transaction> Pending { get; private set; }
        public int Difficulty { get; set; } = 3;
        public decimal Reward { get; set; } = 1m;

        public Blockchain()
        {
            if (File.Exists(FileName))
            {
                var json = File.ReadAllText(FileName);
                Chain = JsonSerializer.Deserialize<List<Block>>(json)!;
            }
            else
            {
                Chain = new List<Block> { new Block(0, new(), "0") };
                Save();
            }
            Pending = new();
        }

        public void Save()
        {
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FileName, JsonSerializer.Serialize(Chain, opt));
        }

        public Block Latest => Chain.Last();

        public void MinePending(string miner)
        {
            var block = new Block(Chain.Count, Pending, Latest.Hash);
            block.Mine(Difficulty);
            Chain.Add(block);
            Pending = new List<Transaction> { new Transaction("SYSTEM", miner, Reward) };
            Save();
        }

        public void AddTransaction(Transaction tx)
        {
            if (!tx.IsValid()) throw new Exception("Invalid transaction");
            if (GetBalance(tx.Sender) < tx.Amount) throw new Exception("Insufficient balance");
            Pending.Add(tx);
        }

        public decimal GetBalance(string addr)
        {
            decimal bal = 0;
            foreach (var blk in Chain)
            {
                foreach (var tx in blk.Transactions)
                {
                    if (tx.Sender == addr) bal -= tx.Amount;
                    if (tx.Receiver == addr) bal += tx.Amount;
                }
            }
            foreach (var tx in Pending)
            {
                if (tx.Sender == addr) bal -= tx.Amount;
                if (tx.Receiver == addr) bal += tx.Amount;
            }
            return bal;
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var curr = Chain[i];
                var prev = Chain[i - 1];
                if (curr.Hash != curr.ComputeHash()) return false;
                if (curr.PreviousHash != prev.Hash) return false;
                if (curr.Transactions.Any(t => !t.IsValid())) return false;
            }
            return true;
        }
    }

}