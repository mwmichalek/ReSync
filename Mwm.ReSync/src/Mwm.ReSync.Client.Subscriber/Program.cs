
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
        
        await client.SubscribeClientAsync((TextMessageEvent evt) => Console.WriteLine($"TextMessage: {evt.Text} {evt.TimeStamp} {evt.Source}"));
        await client.SubscribeServerAsync((UserConnectedEvent evt) => Console.WriteLine($"ServerEvent: Connected: {evt.UserName}"));
        
        client.ServerMessageReceived += eventArgs =>
        { 
            Console.WriteLine($"Subscriber: ServerMessageReceived - {eventArgs.Message}");
            return Task.CompletedTask;
        };
        Console.WriteLine("Subscriber: Subscribed.");
        Console.Read();
    }
}

public class RootObject
{
    public Claims claims { get; set; }
    public Query query { get; set; }
    public Headers headers { get; set; }
    public string[] subprotocols { get; set; }
    public object[] clientCertificates { get; set; }
}

public class Claims
{
    public string[] http___schemas_xmlsoap_org_ws_2005_05_identity_claims_nameidentifier { get; set; }
    public string[] http___schemas_microsoft_com_ws_2008_06_identity_claims_role { get; set; }
    public string[] nbf { get; set; }
    public string[] exp { get; set; }
    public string[] iat { get; set; }
    public string[] aud { get; set; }
    public string[] sub { get; set; }
    public string[] role { get; set; }
}

public class Query
{
    public string[] access_token { get; set; }
}

public class Headers
{
    public string[] Connection { get; set; }
    public string[] Host { get; set; }
    public string[] Upgrade { get; set; }
    public string[] X_Forwarded_Proto { get; set; }
    public string[] X_Forwarded_Host { get; set; }
    public string[] X_Request_ID { get; set; }
    public string[] X_Real_IP { get; set; }
    public string[] X_Forwarded_For { get; set; }
    public string[] X_Forwarded_Port { get; set; }
    public string[] X_Original_URI { get; set; }
    public string[] X_Scheme { get; set; }
    public string[] Sec_WebSocket_Key { get; set; }
    public string[] Sec_WebSocket_Version { get; set; }
    public string[] Sec_WebSocket_Protocol { get; set; }
}

