using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blockchain.Core;
using System.Net.Http.Json;

namespace MinerService
{
    public static class RetrySubmit
    {
        public static async Task SubmitWithFallbackAsync(
            IEnumerable<string> peers,
            Block block)
        {
            foreach (var peer in peers)
            {
                using var client = new HttpClient { BaseAddress = new Uri(peer) };
                try
                {
                    var resp = await client.PostAsJsonAsync("/blocks", block);
                    if (resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Submitted to {peer}");
                        return;
                    }
                }
                catch
                {
                    // Try next peer
                }
            }

            Console.WriteLine("All peers failed to accept the block.");
        }
    }
}