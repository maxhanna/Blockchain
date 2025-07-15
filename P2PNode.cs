using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Yarn
{
    public class P2PNode
    {
        private readonly Blockchain _chain;
        private readonly HttpListener _server;
        private readonly HashSet<string> _peers;

        public P2PNode(Blockchain chain, int port)
        {
            _chain = chain;
            _server = new HttpListener();
            _server.Prefixes.Add($"http://*:{port}/");
            _peers = new HashSet<string>();
        }

        public void Start()
        {
            _server.Start();
            Console.WriteLine("[P2P] Listening for peers...");
            Task.Run(() => ListenLoop());
        }

        private async Task ListenLoop()
        {
            while (true)
            {
                var ctx = await _server.GetContextAsync();
                var req = ctx.Request;
                var resp = ctx.Response;
                string path = req.Url.AbsolutePath;
                if (path == "/chain" && req.HttpMethod == "GET")
                {
                    var data = JsonSerializer.Serialize(_chain.Chain);
                    await Write(resp, data);
                }
                else if (path == "/tx" && req.HttpMethod == "POST")
                {
                    var tx = await JsonSerializer.DeserializeAsync<Transaction>(req.InputStream)!;
                    _chain.AddTransaction(tx);
                    Broadcast("/tx", tx);
                    await Write(resp, "OK");
                }
                else if (path == "/mine" && req.HttpMethod == "POST")
                {
                    string miner = new StreamReader(req.InputStream).ReadToEnd();
                    _chain.MinePending(miner);
                    BroadcastChain();
                    await Write(resp, "Mined");
                }
                else if (path == "/peers")
                {
                    if (req.HttpMethod == "GET")
                    {
                        await Write(resp, JsonSerializer.Serialize(_peers));
                    }
                    else if (req.HttpMethod == "POST")
                    {
                        string peer = new StreamReader(req.InputStream).ReadToEnd();
                        _peers.Add(peer);
                        await Write(resp, "OK");
                    }
                }
                else
                {
                    resp.StatusCode = 404;
                    await Write(resp, "Not Found");
                }
                resp.Close();
            }
        }

        private async Task Write(HttpListenerResponse resp, string data)
        {
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(data);
            await resp.OutputStream.WriteAsync(buf, 0, buf.Length);
        }

        public void AddPeer(string peer)
        {
            _peers.Add(peer);
        }

        public void BroadcastChain()
        {
            Broadcast("/chain", _chain.Chain);
        }

        private void Broadcast<T>(string endpoint, T obj)
        {
            foreach (var peer in _peers)
            {
                try
                {
                    var url = $"http://{peer}{endpoint}";
                    var json = JsonSerializer.Serialize(obj);
                    var wc = new WebClient();
                    wc.UploadString(url, "POST", json);
                }
                catch { }
            }
        }
    }

}
