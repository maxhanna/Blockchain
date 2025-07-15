using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blockchain.Core;


namespace MinerService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Use provided node URL or default to your seed node
            var nodeUrl = args.Length > 0
                ? args[0]
                : "http://142.112.110.151:52345";

            var bc = new Blockchain.Core.Blockchain(); 
            bc.ConnectPeer("http://142.112.110.151:52345");
            using var client = new HttpClient { BaseAddress = new Uri(nodeUrl) };
            var bestPeer = await PeerSelector.ChooseBestPeerAsync(bc.Peers);
            client.BaseAddress = new Uri(bestPeer);
            Console.WriteLine($"Miner connecting to node at {client.BaseAddress}");

            while (true)
            {
                var block = bc.MineBlock();
                Console.WriteLine($"Mined block {block.Index} with nonce {block.Nonce}");
                try
                {
                    var response = await client.PostAsJsonAsync("/blocks", block);
                    Console.WriteLine(response.IsSuccessStatusCode ? "Submitted!" : "Rejected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error submitting block to {nodeUrl}: {ex.Message}");
                    await RetrySubmit.SubmitWithFallbackAsync(bc.Peers, block); 
                    await Task.Delay(TimeSpan.FromSeconds(1));

                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}