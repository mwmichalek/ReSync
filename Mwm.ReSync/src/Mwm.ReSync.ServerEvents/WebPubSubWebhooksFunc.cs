using System.Text;
using System.Text.Json;
using Azure;
using Azure.Messaging.WebPubSub;
using Azure.Messaging.WebPubSub.Clients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Mwm.ReSync.ServerEvents.Extensions;
using Newtonsoft.Json;

namespace Mwm.ReSync.ServerEvents;

public class WebPubSubWebhooksFunc
{
    private readonly ILogger<WebPubSubWebhooksFunc> _logger;
    private readonly string _connectionString;
    private readonly string _hubName;
    public WebPubSubWebhooksFunc(ILogger<WebPubSubWebhooksFunc> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["AzureWebJobsConfiguration:ConnectionString"];
        _hubName = configuration["AzureWebJobsConfiguration:HubName"];
    }
    
    [Function("WebPubSubEventOccured")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest request)
    {
        // Read the WebHook-Request-Origin header
        var requestOrigin = request.Headers["WebHook-Request-Origin"];
 
        if (request.Method == HttpMethods.Get && !string.IsNullOrEmpty(requestOrigin))
        {
            request.HttpContext.Response.Headers.Append("WebHook-Allowed-Origin", requestOrigin);
            _logger.LogInformation($"Authorization Event.");
            return new OkResult();
        }
        
        var requestId = request.Headers["x-ms-client-request-id"];
        var userId = request.Headers["ce-userId"];
        var eventType = request.Headers["ce-eventName"];
        
        var serviceClient = new WebPubSubServiceClient(_connectionString, _hubName);
        
        var uri = serviceClient.GetClientAccessUri(
            userId: "server", 
            roles: new string[]
            {
                "webpubsub.joinLeaveGroup", 
                "webpubsub.sendToGroup"
            });
        
        var client = new WebPubSubClient(uri); 
        await client.StartAsync(); 
        _logger.LogInformation($"Connected to Web PubSub.");
        
        
        //await client.PublishAsync(new TextMessageEvent { Text = streaming });
        
        //TODO: Create eventType and publish

        return new OkResult();
    }
    
    
    
    // // Read and log the request details
    // var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    // var headers = request.Headers;
    // var queryString = request.QueryString.ToString();
    //
    // // Build a diagnostic string
    // var diagnostics = new StringBuilder();
    // diagnostics.AppendLine("Request Body:");
    // diagnostics.AppendLine(requestBody);
    // diagnostics.AppendLine("Headers:");
    // foreach (var header in headers)
    // {
    //     diagnostics.AppendLine($"{header.Key}: {header.Value}");
    // }
    // diagnostics.AppendLine("Query String:");
    // diagnostics.AppendLine(queryString);
    //
    // // Log the diagnostic information
    // _logger.LogInformation(diagnostics.ToString());
    //     
        
        
    
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