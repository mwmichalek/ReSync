using System.Text.Json;
using Azure.Messaging.WebPubSub.Clients;
using Mwm.ReSync.Client.Common;

namespace Mwm.ReSync.Lib;

public static class WebPubSubClientExtensions
{
    private static readonly IDictionary<string, IEventSubscription> EventSubscriptions = new Dictionary<string, IEventSubscription>();
    
    public static async Task SubscribeAsync<TEvent>(this WebPubSubClient client, Action<TEvent> onEvent) where TEvent : Event
    {
        await client.JoinGroupAsync(typeof(TEvent).Name);
        
        // Only register the main function once
        if (EventSubscriptions.Count == 0)
        {
            client.GroupMessageReceived += (eventArgs) =>
            {
                if (EventSubscriptions.TryGetValue(eventArgs.Message.Group, out IEventSubscription subscription))
                {
                    var messageJson = eventArgs.Message.Data.ToString();
                    subscription.HandleEvent(messageJson);
                }

                return Task.CompletedTask;
            };
            
        }
        
        EventSubscriptions[typeof(TEvent).Name] = new EventSubscription<TEvent>(onEvent);
    }
    
    //private static readonly IDictionary<string, IEventSubscription> ServerEventSubscriptions = new Dictionary<string, IEventSubscription>();
    
    // public static async Task SubscribeServerAsync<TEvent>(this WebPubSubClient client, Action<TEvent> onEvent) where TEvent : ServerEvent
    // {
    //
    //     // Only register the main function once
    //     if (ServerEventSubscriptions.Count == 0)
    //     {
    //         client.ServerMessageReceived += (eventArgs) =>
    //         {
    //             // TODO: How do we look up the server event type?
    //             if (ServerEventSubscriptions.TryGetValue(eventArgs.GetType().Name, out IEventSubscription subscription))
    //             {
    //                 var messageJson = eventArgs.Message.Data.ToString();
    //                 subscription.HandleEvent(messageJson);
    //             }
    //
    //             return Task.CompletedTask;
    //         };
    //     }
    //     
    //     ServerEventSubscriptions[typeof(TEvent).Name] = new EventSubscription<TEvent>(onEvent);
    // }
    
    
    public static async Task PublishAsync<TEvent>(this WebPubSubClient client, TEvent @event) where TEvent : Event
    {
        await client.SendToGroupAsync(typeof(TEvent).Name, BinaryData.FromObjectAsJson(@event), WebPubSubDataType.Json);
    }
}

public interface IEventSubscription {

    Task HandleEvent(string eventBody);

}

public class EventSubscription<TEvent> : IEventSubscription where TEvent : Event {

    private Action<TEvent> _eventHandler;

    public EventSubscription(Action<TEvent> eventHandler) => _eventHandler = eventHandler;

    public Task HandleEvent(string eventBody) {
        var evt = JsonSerializer.Deserialize<TEvent>(eventBody);
        if (evt != null)
            _eventHandler(evt);
        return Task.CompletedTask;
    }
}