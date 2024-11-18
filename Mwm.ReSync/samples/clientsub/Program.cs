using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.WebPubSub;
using Azure.Messaging.WebPubSub.Clients;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Azure.WebPubSub.Common;
using Mwm.ReSync.Clients;
using Websocket.Client;

namespace clientsub;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: clientsub <connectionString> <hub>");
            return;
        }
        var connectionString = args[0];
        var hub = args[1];

        // Either generate the URL or fetch it from server or fetch a temp one from the portal
        var serviceClient = new WebPubSubServiceClient(connectionString, hub);
        
        //ExpiringMessage
        //TranslatedMessage
        var uri = serviceClient.GetClientAccessUri(
            userId: "subscriber", 
            roles: new string[]
            {
                "webpubsub.joinLeaveGroup.ExpiringMessage", 
                "webpubsub.sendToGroup.ExpiringMessage",
                "webpubsub.joinLeaveGroup.TranslatedMessage",
                "webpubsub.sendToGroup.TranslatedMessage"
            });
        
        var client = new WebPubSubClient(uri);
        await client.StartAsync();
        Console.WriteLine("Subscriber: Connected.");
        
        await client.SubscribeAsync((ExpiringMessage msg) => Console.WriteLine($"ExpiringMessage: {msg.Body} {msg.ExpirationTime} {msg.TimeStamp}"));
        await client.SubscribeAsync((TranslatedMessage msg) => Console.WriteLine($"TranslatedMessage: {msg.Body} {msg.TranslatedText}  {msg.TimeStamp}"));
        

        Console.WriteLine("Subscriber: Subscribed.");
        Console.Read();
        
        // using (var client = new WebsocketClient(uri, () =>
        // {
        //     var inner = new ClientWebSocket();
        //     inner.Options.AddSubProtocol("json.webpubsub.azure.v1");
        //     return inner;
        // }))
        // {
        //     // Disable the auto disconnect and reconnect because the sample would like the client to stay online even no data comes in
        //
        //     client.ReconnectTimeout = null;
        //     //client.MessageReceived.Subscribe(msg => Console.WriteLine($"Message received: {msg}"));
        //     client.Subscribe((ExpiringMessage msg) => Console.WriteLine($"ExpiringMessage: {msg.Body} {msg.ExpirationTime} {msg.TimeStamp}"));
        //     client.Subscribe((TranslatedMessage msg) => Console.WriteLine($"TranslatedMessage: {msg.Body} {msg.TranslatedText}  {msg.TimeStamp}"));
        //     
        //     await client.Start();
        //     Console.WriteLine("Connected.");
        //     client.Send(JsonSerializer.Serialize(new
        //     {
        //         type = "joinGroup",
        //         group = "demogroup",
        //         ackId = 1
        //     }));
        //     Console.Read();
        // }
    }
}

