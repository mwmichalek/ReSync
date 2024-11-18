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
                "webpubsub.joinLeaveGroup.ExpiringMessage", 
                "webpubsub.sendToGroup.ExpiringMessage",
                "webpubsub.joinLeaveGroup.TranslatedMessage",
                "webpubsub.sendToGroup.TranslatedMessage"
            });
        
        var client = new WebPubSubClient(uri); 
        await client.StartAsync(); 
        Console.WriteLine("Publisher: Connected.");
        
        
        var streaming = Console.ReadLine();
        while (streaming != null)
        {
            
            
            await client.PublishAsync(new ExpiringMessage { Body = streaming , ExpirationTime = DateTime.Now.AddDays(7) , TimeStamp = DateTime.Now });
            await client.PublishAsync(new TranslatedMessage { Body = streaming , TranslatedText = streaming.ToLower() , TimeStamp = DateTime.Now });
            Console.WriteLine("Publisher: Published messages.");
            
            streaming = Console.ReadLine();
        }
        
        Console.WriteLine("Done.");
        
        // using (var client = new WebsocketClient(url, () =>
        // {
        //     var inner = new ClientWebSocket();
        //     inner.Options.AddSubProtocol("json.webpubsub.azure.v1");
        //     return inner;
        // }))
        // {
        //     // Disable the auto disconnect and reconnect because the sample would like the client to stay online even no data comes in
        //     client.ReconnectTimeout = null;
        //     
        //     //client.MessageReceived.Subscribe(msg => Console.WriteLine($"Message received: {msg}"));
        //     
        //     await client.Start();
        //     Console.WriteLine("Connected.");
        //     /* Send to group `demogroup` */
        //     int ackId = 1;
        //     var streaming = Console.ReadLine();
        //     while (streaming != null)
        //     {
        //         //client.Publish(new ExpiringMessage { Body = streaming , ExpirationTime = DateTime.Now.AddDays(7) , TimeStamp = DateTime.Now });
        //         //client.Publish(new TranslatedMessage { Body = streaming , TranslatedText = streaming.ToLower() , TimeStamp = DateTime.Now });
        //         //client.Se
        //         
        //         
        //         // client.Send(JsonSerializer.Serialize(new
        //         // {
        //         //     type = "sendToGroup",
        //         //     group = "demogroup",
        //         //     dataType = "text",
        //         //     data = streaming,
        //         //     ackId = ackId++
        //         // }));
        //         streaming = Console.ReadLine();
        //     }
        //
        //     Console.WriteLine("done");
        //     /*  ------------  */
        // }
    }
}


