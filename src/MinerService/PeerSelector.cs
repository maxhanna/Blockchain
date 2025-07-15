using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MinerService
{
    public static class PeerSelector
    {
        public static async Task<string> ChooseBestPeerAsync(IEnumerable<string> peers)
        {
            var client = new HttpClient();
            var timings = new Dictionary<string, long>();

            foreach (var peer in peers)
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await client.GetAsync($"{peer}/ping");
                }
                catch
                {
                    sw.Stop();
                    timings[peer] = long.MaxValue;
                    continue;
                }
                sw.Stop();
                timings[peer] = sw.ElapsedMilliseconds;
            }
            if (!timings.Any())
                return peers.First();

            return timings.OrderBy(kv => kv.Value).First().Key;
        }
    }
}