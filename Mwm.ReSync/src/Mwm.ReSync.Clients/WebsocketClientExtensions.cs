using System.Text.Json;
using Websocket.Client;

namespace Mwm.ReSync.Clients;

public static class WebsocketClientExtensions
{
    private static readonly IDictionary<string, IMessageSubscription> MessageSubscriptions = new Dictionary<string, IMessageSubscription>();
    public static void Subscribe<TMessage>(this WebsocketClient websocket, Action<TMessage> onMessage) where TMessage : class
    {
        MessageSubscriptions[typeof(TMessage).Name] = new MessageSubscription<TMessage>(onMessage);
        websocket.MessageReceived.Subscribe(msg => {
            try
            {
                var typedMessage = JsonSerializer.Deserialize<TypedMessage>(msg.Text);
                var messagedSubscription = MessageSubscriptions[typedMessage.MessageType];
                messagedSubscription.HandleNotification(typedMessage.MessageJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    public static void Publish<TMessage>(this WebsocketClient websocket, TMessage message) where TMessage : class
    {
        var typedMessage = new TypedMessage
            { MessageType = typeof(TMessage).Name, 
                MessageJson = JsonSerializer.Serialize(message) };
        var typedMessageJson = JsonSerializer.Serialize(typedMessage);
        websocket.Publish(typedMessageJson);
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

public class TypedMessage
{
    public string MessageType { get; set; }
    
    public string MessageJson { get; set; }
}