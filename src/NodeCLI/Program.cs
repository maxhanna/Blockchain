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
            app.MapPost("/blocks", (Block block) =>
            {
                if (!bc.ValidChain(bc.Chain.Append(block))) return Results.BadRequest();
                bc.Chain.Add(block);
                return Results.Ok();
            });

            if (args.Length > 0 && args[0] == "start")
            {
                Console.WriteLine("Starting node on http://localhost:5000");
                app.Run("http://localhost:5000");
            }
            else
            {
                foreach (var block in bc.Chain)
                    Console.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            }
        }
    }
}