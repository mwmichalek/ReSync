using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Mwm.ReSync.ServerEvents.Extensions;

namespace Mwm.ReSync.ServerEvents;

public class WebPubSubWebhooksFunc
{
    private readonly ILogger<WebPubSubWebhooksFunc> _logger;

    public WebPubSubWebhooksFunc(ILogger<WebPubSubWebhooksFunc> logger)
    {
        _logger = logger;
    }
    
    [Function("WebPubSubEventOccured")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request)
    {
        // Read the WebHook-Request-Origin header
        var requestOrigin = request.Headers["WebHook-Request-Origin"];
        request.HttpContext.Response.Headers.Append("WebHook-Allowed-Origin", requestOrigin);
        
        if (!string.IsNullOrEmpty(requestOrigin))
        {
            _logger.LogInformation("Authorization Event");
            return new OkObjectResult("I approve of this message");
        }

        var body = await request.GetRawBodyStringAsync();
        _logger.LogInformation($"C# HTTP trigger function processed a request : {body}");
        return new OkObjectResult($"Welcome to Azure Functions! : {body}");
    }
    
    // [FunctionName("WebPubSubEventOccured")]
    // public async Task<IActionResult> Run(
    //     [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request, ILogger log) {
    //     
    //     log.LogInformation("C# HTTP trigger function processed a request.");
    //     
    //     request.Body
    //     //var body = await GetRawBodyStringAsync(request);
    //
    //     if (body.Contains("challenge")) {
    //         log.LogInformation($"You like my body? {body}");
    //     }
    //     
    //     return new ContentResult {
    //         Content = body,
    //         ContentType = "application/json"
    //     };
    // }
   
    
    // [FunctionName("WebPubSubTrigger")]
    // public static void Run(
    //     [WebPubSubTrigger("messages", WebPubSubEventType.User, "message")] UserEventRequest request, ILogger log)
    // {
    //     log.LogInformation($"Request from: {request.ConnectionContext.UserId}");
    //     log.LogInformation($"Request message data: {request.Data}");
    //     log.LogInformation($"Request message dataType: {request.DataType}");
    // }

    // [FunctionName("WebPubSubFunction")] 
    // public static async Task Run( 
    //     [WebPubSubTrigger("myHub", WebPubSubEventType.System, "connected")] WebPubSubConnectionContext context, 
    //     ILogger log, [WebPubSub(Hub = "myHub")] IAsyncCollector<WebPubSubOperation> operations) { 
    //     log.LogInformation($"Client connected: {context.ConnectionId}"); 
    //     var message = new { type = "newConnection", connectionId = context.ConnectionId, userId = context.UserId }; 
    //     await operations.AddAsync(new SendToAll { Data = BinaryData.FromObjectAsJson(message), DataType = WebPubSubDataType.Json });
}