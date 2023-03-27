using System.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Net7Bot.Sqlite;

public class User
{
    [PrimaryKey, NotNull] public string Id { get; set; }
    public string Username { get; set; }
    [DefaultValue(0)] public long TotalMessageCount { get; set; } = 0;

    [ManyToMany(typeof(ServerUserRelationship))]
    public List<Server> Servers { get; set; } = new();
}