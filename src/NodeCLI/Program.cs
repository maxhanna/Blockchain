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
                bc.Chain.Add(block);
                return Results.Ok();
            });


            if (args.Length > 0 && args[0] == "start")
            {
                Console.WriteLine("Starting node on http://localhost:52345");
                app.Run("http://localhost:52345");
            }
            else
            {
                foreach (var block in bc.Chain)
                    Console.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            }
            bc.ConnectPeer("http://142.112.110.151:52345"); 
        }
    }
}