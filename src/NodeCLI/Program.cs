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
        static void Main(string[] args)
        {
            var bc = new Blockchain.Core.Blockchain();
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
                if (!bc.ValidChain(bc.Chain.Append(block))) return Results.BadRequest();
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