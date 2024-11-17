using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.WebPubSub;

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
        var url = serviceClient.GetClientAccessUri(userId: "user1", roles: new string[] {"webpubsub.joinLeaveGroup.demogroup", "webpubsub.sendToGroup.demogroup"});

        using (var client = new WebsocketClient(url, () =>
        {
            var inner = new ClientWebSocket();
            inner.Options.AddSubProtocol("json.webpubsub.azure.v1");
            return inner;
        }))
        {
            // Disable the auto disconnect and reconnect because the sample would like the client to stay online even no data comes in
            client.ReconnectTimeout = null;
            
            client.MessageReceived.Subscribe(msg => Console.WriteLine($"Message received: {msg}"));
            //client.MessageReceived.s
            //client.MessageReceived.
            
            await client.Start();
            Console.WriteLine("Connected.");
            /* Send to group `demogroup` */
            int ackId = 1;
            var streaming = Console.ReadLine();
            while (streaming != null)
            {
                client.Send(JsonSerializer.Serialize(new
                {
                    type = "sendToGroup",
                    group = "demogroup",
                    dataType = "text",
                    data = streaming,
                    ackId = ackId++
                }));
                streaming = Console.ReadLine();
            }

            Console.WriteLine("done");
            /*  ------------  */
        }
    }
}

public static class WebsocketClientExtensions
{
    
    private static IDictionary<string, Action<IMessage>> _handlers = new Dictionary<string, Action<IMessage>>();
    public static void Subscribe<TMessage>(this WebsocketClient websocket, Action<TMessage> onMessage) where TMessage : IMessage
    {
        _handlers[typeof(TMessage).Name] = onMessage;
        websocket.MessageReceived.Subscribe(msg => {
            var groupMessage = JsonSerializer.Deserialize<GroupMessage>(msg.Text);
            
        });
    }
}


public class ExpiringMessage : Message
{
    public DateTime ExpirationTime { get; set; }
}

public class TranslatedMessage : Message
{
    public string TranslatedText { get; set; }
}

public class Message
{
    public string Body { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
}

public interface IMessage
{
    
}

public class GroupMessage
{
    public string GroupName { get; set; }
    
    public string JsonMessage { get; set; }
}

