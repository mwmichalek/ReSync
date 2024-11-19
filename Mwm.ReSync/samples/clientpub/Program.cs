using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.WebPubSub;
using Azure.Messaging.WebPubSub.Clients;
using Mwm.ReSync.Clients;
using Websocket.Client;

namespace clientpub;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: clientpub <connectionString> <hub>");
            return;
        }
        var connectionString = args[0];
        var hub = args[1];

        // Either generate the URL or fetch it from server or fetch a temp one from the portal
        var serviceClient = new WebPubSubServiceClient(connectionString, hub);
        
        var uri = serviceClient.GetClientAccessUri(
            userId: "publisher", 
            roles: new string[]
            {
                "webpubsub.joinLeaveGroup", 
                "webpubsub.sendToGroup"
            });
        
        var client = new WebPubSubClient(uri); 
        await client.StartAsync(); 
        Console.WriteLine("Publisher: Connected.");
        
        client.ServerMessageReceived += eventArgs =>
        { 
            Console.WriteLine($"Publisher: ServerMessageReceived - {eventArgs.Message}");
            return Task.CompletedTask;
        };
        
        var streaming = Console.ReadLine();
        while (streaming != null)
        {
            await client.PublishAsync(new ExpiringMessage { Body = streaming , ExpirationTime = DateTime.Now.AddDays(7) , TimeStamp = DateTime.Now });
            await client.PublishAsync(new TranslatedMessage { Body = streaming , TranslatedText = streaming.ToLower() , TimeStamp = DateTime.Now });
            Console.WriteLine("Publisher: Published messages.");
            
            streaming = Console.ReadLine();
        }
        
        Console.WriteLine("Done.");
    }
}


