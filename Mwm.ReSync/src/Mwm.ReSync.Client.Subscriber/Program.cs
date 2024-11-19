
using Mwm.ReSync.Client.Common;

namespace Mwm.ReSync.Client.Subscriber;
using System;

using System.Threading.Tasks;

using Azure.Messaging.WebPubSub;
using Azure.Messaging.WebPubSub.Clients;
// using Microsoft.AspNetCore.Mvc.TagHelpers;
// using Microsoft.Azure.WebPubSub.Common;
using Lib;

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
        
        var uri = serviceClient.GetClientAccessUri(
            userId: "subscriber", 
            roles: new string[]
            {
                "webpubsub.joinLeaveGroup", 
                "webpubsub.sendToGroup"
            });
        
        var client = new WebPubSubClient(uri);
        await client.StartAsync();
        Console.WriteLine("Subscriber: Connected.");
        
        await client.SubscribeAsync((ExpiringMessage msg) => Console.WriteLine($"ExpiringMessage: {msg.Body} {msg.ExpirationTime} {msg.TimeStamp}"));
        await client.SubscribeAsync((TranslatedMessage msg) => Console.WriteLine($"TranslatedMessage: {msg.Body} {msg.TranslatedText}  {msg.TimeStamp}"));

        client.ServerMessageReceived += eventArgs =>
        { 
            Console.WriteLine($"Subscriber: ServerMessageReceived - {eventArgs.Message}");
            return Task.CompletedTask;
        };
        Console.WriteLine("Subscriber: Subscribed.");
        Console.Read();
    }
}