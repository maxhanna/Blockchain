using System;
using System.Collections.Generic;

namespace MinerService
{
    public static class LoadBalancer
    {
        private static readonly Random _rand = new();
        public static string GetRandomPeer(List<string> peers)
            => peers[_rand.Next(peers.Count)];
    }
}