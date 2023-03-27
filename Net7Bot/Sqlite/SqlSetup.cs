using SQLite;

namespace Net7Bot.Sqlite;

public static class SqlSetup
{
    public static SQLiteAsyncConnection databaseConnection;

    public static async Task InitSqlite()
    {
        databaseConnection = new("../sqliteDatabase.sqlite");
        await databaseConnection.CreateTableAsync<User>();
        await databaseConnection.CreateTableAsync<Server>();
        await databaseConnection.CreateTableAsync<ServerUserRelationship>();
    }
}