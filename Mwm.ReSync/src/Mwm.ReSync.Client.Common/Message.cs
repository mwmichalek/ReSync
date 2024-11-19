namespace Mwm.ReSync.Client.Common;

public class Message
{
    public string Body { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
}

public class ExpiringMessage : Message
{
    public DateTime ExpirationTime { get; set; }
}

public class TranslatedMessage : Message
{
    public string TranslatedText { get; set; }
}
