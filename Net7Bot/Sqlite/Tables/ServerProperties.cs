namespace Net7Bot.Sqlite;

public class ServerProperties
{
    public bool PassiveInteraction { get; set; }
    public bool ShowJoinLeaveMessages { get; set; }

    public ulong WelcomeChannelId { get; set; }

    public string JoinMessage { get; set; } =
        "<a:wave:1053377818790215781> Hello, [user], I hope you have a good stay!";

    public string LeaveMessage { get; set; } =
        "<a:wave:1053377818790215781> Goodbye, [user], I hope you had a good time!";
}