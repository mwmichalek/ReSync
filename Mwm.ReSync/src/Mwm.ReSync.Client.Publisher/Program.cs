using Azure.Messaging.WebPubSub;
using Azure.Messaging.WebPubSub.Clients;
using Mwm.ReSync.Client.Common;
using Mwm.ReSync.Lib;

namespace Mwm.ReSync.Client.Publisher;

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
        
        await client.SubscribeAsync((UserConnectedEvent evt) => Console.WriteLine($"ServerEvent: Connected: {evt.UserName}"));
        await client.SubscribeAsync((UserDisconnectedEvent evt) => Console.WriteLine($"ServerEvent: Disconnected: {evt.UserName}"));

        Console.WriteLine("Publisher: [Type something here....]");
        
        var streaming = Console.ReadLine();
        while (streaming != null)
        {
            await client.PublishAsync(new TextMessageEvent { Text = streaming });
            
            Console.WriteLine("Publisher: Published event.");
            
            streaming = Console.ReadLine();
        }
        
        Console.WriteLine("Done.");
    }
}