using DSharpPlus.Entities;
using Net7Bot.CustomLogging;
using SQLiteNetExtensionsAsync.Extensions;
using static Net7Bot.Sqlite.SqlSetup;

namespace Net7Bot.Sqlite;

public static class ExtensionMethods
{
    public static void Each<T>(this IEnumerable<T> arr, Action<T> each)
    {
        foreach (var e in arr) each(e);
    }
    
    public static async Task<User> AddUser(this DiscordUser user)
    {
        User newUser = new()
        {
            Id = user.Id.ToString(), Username = user.Username, TotalMessageCount = 0
        };
        Logger.Log($"Created User [{newUser.Username}]");

        await databaseConnection.InsertWithChildrenAsync(newUser);
        return newUser;
    }

    public static async Task<Server> AddServer(this DiscordGuild server)
    {
        Server newServer = new()
        {
            Id = server.Id.ToString(), ServerName = server.Name
        };
        Logger.Log($"Created Server [{newServer.ServerName}]");

        await databaseConnection.InsertWithChildrenAsync(newServer);
        return newServer;
    }

    public static async Task<T> FindWithChildren<T>(this object pk, Func<Task<T>> createNew) where T : new()
    {
        return await databaseConnection.FindWithChildrenAsync<T>(pk, true) ?? await createNew();
    }

    public static async Task<User> GetUser(this DiscordUser user)
    {
        if (user.IsBot) return null;
        var id = user.Id.ToString();
        return await id.FindWithChildren(user.AddUser);
    }

    public static async Task<Server> GetServer(this DiscordGuild server)
    {
        var id = server.Id.ToString();
        return await id.FindWithChildren(server.AddServer);
    }

    public static async Task UpdateTable<T>(this T table)
    {
        await databaseConnection.UpdateWithChildrenAsync(table);
    }
}