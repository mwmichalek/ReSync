using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mwm.ReSync.ServerEvents;

public class PublishWebPubSubEvent
{
    private readonly ILogger<PublishWebPubSubEvent> _logger;

    public PublishWebPubSubEvent(ILogger<PublishWebPubSubEvent> logger)
    {
        _logger = logger;
    }

    // [FunctionName("WebPubSubFunction")] 
    // public static async Task Run( 
    //     [WebPubSubTrigger("myHub", WebPubSubEventType.System, "connected")] WebPubSubConnectionContext context, 
    //     ILogger log, [WebPubSub(Hub = "myHub")] IAsyncCollector<WebPubSubOperation> operations) { 
    //     log.LogInformation($"Client connected: {context.ConnectionId}"); 
    //     var message = new { type = "newConnection", connectionId = context.ConnectionId, userId = context.UserId }; 
    //     await operations.AddAsync(new SendToAll { Data = BinaryData.FromObjectAsJson(message), DataType = WebPubSubDataType.Json });
}