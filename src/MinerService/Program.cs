using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blockchain.Core;

namespace MinerService
{
    class Program
    {
        static async Task Main()
        {
            var bc = new Blockchain.Core.Blockchain();
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
            while (true)
            {
                var block = bc.MineBlock();
                Console.WriteLine($"Mined block {block.Index} with nonce {block.Nonce}");
                var response = await client.PostAsJsonAsync("/blocks", block);
                Console.WriteLine(response.IsSuccessStatusCode ? "Submitted!" : "Rejected");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}