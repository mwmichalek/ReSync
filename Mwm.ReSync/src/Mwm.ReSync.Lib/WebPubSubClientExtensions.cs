using System.Text.Json;
using Azure.Messaging.WebPubSub.Clients;
using Websocket.Client;

namespace Mwm.ReSync.Lib;

public static class WebPubSubClientExtensions
{
    private static readonly IDictionary<string, IMessageSubscription> MessageSubscriptions = new Dictionary<string, IMessageSubscription>();
    
    public static async Task SubscribeAsync<TMessage>(this WebPubSubClient client, Action<TMessage> onMessage) where TMessage : class
    {
        await client.JoinGroupAsync(typeof(TMessage).Name);
        
        // Only register the main function once
        if (MessageSubscriptions.Count == 0)
        {
            client.GroupMessageReceived += (messageArgs) =>
            {
                if (MessageSubscriptions.TryGetValue(messageArgs.Message.Group, out IMessageSubscription subscription))
                {
                    var messageJson = messageArgs.Message.Data.ToString();
                    subscription.HandleNotification(messageJson);
                }

                return Task.CompletedTask;
            };
            
        }
        
        MessageSubscriptions[typeof(TMessage).Name] = new MessageSubscription<TMessage>(onMessage);
    }

    public static async Task PublishAsync<TMessage>(this WebPubSubClient client, TMessage message) where TMessage : class
    {
        await client.SendToGroupAsync(typeof(TMessage).Name, BinaryData.FromObjectAsJson(message), WebPubSubDataType.Json);
    }
}

public interface IMessageSubscription {

    Task HandleNotification(string notificationBody);

}

public class MessageSubscription<TMessage> : IMessageSubscription where TMessage : class {

    private Action<TMessage> _messageHandler;

    public MessageSubscription(Action<TMessage> messageHandler) => _messageHandler = messageHandler;

    public Task HandleNotification(string messageBody) {
        var message = JsonSerializer.Deserialize<TMessage>(messageBody);
        if (message != null)
            _messageHandler(message);
        return Task.CompletedTask;
    }
}