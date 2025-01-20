namespace Mwm.ReSync.Client.Common;

public abstract class Event
{
    public string? Source { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}

public abstract class ClientEvent : Event
{
    
}

public class TextMessageEvent : ClientEvent
{
    public required string Text { get; set; }
}

public abstract class ServerEvent : Event
{
    public Guid RequestId { get; set; }
}

public class UserConnectedEvent : ServerEvent
{
    public string? UserName { get; set; }
}

public class UserDisconnectedEvent : ServerEvent
{
    public string? UserName { get; set; }
}

public class UserReconnectEvent : ServerEvent
{
    public string? UserName { get; set; }
}
