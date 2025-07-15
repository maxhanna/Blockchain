using System;

namespace Yarn
{
    public class Program
    {
        private const int DefaultPort = 52345; // Rarely used static port

        public static void Main(string[] args)
        {
            int port = DefaultPort;
            var bc = new Blockchain();
            var node = new P2PNode(bc, port);

            // Default bootstrap peer: localhost on static port
            node.AddPeer($"127.0.0.1:{port}");
            // Additional peers from args
            foreach (var a in args) if (a.Contains(':')) node.AddPeer(a);

            node.Start();

            var wallet = new Wallet();
            Console.WriteLine($"Your address: {wallet.Address}");
            Console.WriteLine($"Listening on port {port}");
            Console.WriteLine("Commands: balance, send <addr> <amt>, mine");

            while (true)
            {
                var line = Console.ReadLine()!;
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;
                switch (parts[0])
                {
                    case "balance":
                        Console.WriteLine($"Balance: {bc.GetBalance(wallet.Address)}");
                        break;
                    case "send":
                        if (parts.Length != 3) { Console.WriteLine("Usage: send <addr> <amt>"); break; }
                        var tx = new Transaction(wallet.Address, parts[1], decimal.Parse(parts[2]));
                        tx.Sign(wallet.PrivatePem);
                        bc.AddTransaction(tx);
                        node.BroadcastChain();
                        break;
                    case "mine":
                        bc.MinePending(wallet.Address);
                        node.BroadcastChain();
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }
    }

}

