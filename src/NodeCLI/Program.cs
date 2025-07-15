using System;
using Blockchain.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using static Newtonsoft.Json.Formatting;

namespace NodeCLI
{
    class Program
    {
        async static void Main(string[] args)
        {
            var chainFile = "chain.json";
            var storage = new FileStorage(chainFile);
 
            var bc = new Blockchain.Core.Blockchain(difficulty: 4, storage: storage);

            // attempt to fetch chain from peers
            var peers = new PeerStorage().Load();

            if (!bc.Chain.Any() && peers.Any())
            {
                using var client = new HttpClient();

                foreach (var peer in peers)
                {
                    try
                    {
                        var remoteChain = await client.GetFromJsonAsync<List<Block>>($"{peer}/chain");
                        if (remoteChain != null && bc.ReplaceChain(remoteChain))
                        {
                            Console.WriteLine($"Imported chain from peer {peer}");
                            break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to contact {peer}");
                    }
                }
            }

            // If still no chain, mine genesis
            if (!bc.Chain.Any())
            {
                Console.WriteLine("No chain found on any peer — mining genesis block.");
                bc.MineGenesisBlock();  
            }
            
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.MapGet("/chain", () => bc.Chain);
            app.MapGet("/balance/{address}", (string address) =>
            {
                var balance = bc.GetBalance(address);
                return Results.Ok(new { Address = address, Balance = balance });
            });
            app.MapPost("/blocks", (Block block) =>
            {
                var testChain = bc.Chain.ToList();  
                testChain.Add(block);

                if (!bc.ValidChain(testChain))
                {
                    Console.WriteLine("Rejected block: invalid chain");
                    return Results.BadRequest();
                }
                bc.AddBlock(block);
                return Results.Ok("Block added and saved");
            });


            if (args.Length > 0 && args[0] == "start")
            {
                Console.WriteLine("Starting node on http://0.0.0.0:52345");
                app.Run("http://0.0.0.0:52345");
            }
            else
            {
                foreach (var block in bc.Chain)
                    Console.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            }
            var peerStorage = new PeerStorage();
            bc.Peers = peerStorage.Load();      // load saved peers
            bc.ConnectPeer("http://142.112.110.151:52345");
            peerStorage.Save(bc.Peers);
        }
    }
}