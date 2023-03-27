using Net7Bot.CustomLogging;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensionsAsync.Extensions;
using static Net7Bot.Sqlite.SqlSetup;

namespace Net7Bot.Sqlite;

public class Server
{
    [PrimaryKey, NotNull] public string Id { get; set; }
    public string ServerName { get; set; }

    public string SettingsBlobbed { get; set; }
    public string CacheBlobbed { get; set; }

    [TextBlob(nameof(SettingsBlobbed))] public ServerProperties Settings { get; set; } = new();
    [TextBlob(nameof(CacheBlobbed))] public ServerCache Cache { get; set; } = new();

    [ManyToMany(typeof(ServerUserRelationship))]
    public List<User> Users { get; set; } = new();

    public async Task<bool> CheckAddUser(User user)
    {
        if (await HasUser(user)) return false;
        await AddUser(user);
        Logger.Log($"Added [{user.Username}] to server [{ServerName}]");
        return true;
    }

    public async Task<bool> HasUser(User user)
    {
        var relationships = await databaseConnection.GetAllWithChildrenAsync<ServerUserRelationship>();
        return relationships.Any(relationship => relationship.ServerId == Id && relationship.UserId == user.Id);
    }

    public async Task AddUser(User user)
    {
        if (await HasUser(user)) return;
        await databaseConnection.InsertAsync(new ServerUserRelationship { ServerId = Id, UserId = user.Id });
        await databaseConnection.GetChildrenAsync(this);
    }

    public async Task RemoveUser(User user)
    {
        if (!await HasUser(user)) return;
        await databaseConnection.DeleteAsync(new ServerUserRelationship { ServerId = Id, UserId = user.Id });
        await databaseConnection.GetChildrenAsync(this);
    }
}