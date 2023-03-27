using SQLiteNetExtensions.Attributes;

namespace Net7Bot.Sqlite;

public class ServerUserRelationship
{
    [ForeignKey(typeof(Server))] public string ServerId { get; set; }
    [ForeignKey(typeof(User))] public string UserId { get; set; }
}