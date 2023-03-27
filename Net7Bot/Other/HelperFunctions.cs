using DSharpPlus.Entities;
using Newtonsoft.Json;
using static Net7Bot.Program;

namespace Net7Bot.Other;

public static class HelperFunctions
{
    /// <summary>
    /// yoink some json from the web
    /// </summary>
    /// <param name="site">website url</param>
    /// <returns>the yoinked data</returns>
    public static async Task<string> LoadFromWeb(this string site)
    {
        try
        {
            var client = new HttpClient();
            using var response = await client.GetAsync(site);
            using var content = response.Content;
            return await content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static async Task<T> LoadJsonFromString<T>(this string data)
    {
        return JsonConvert.DeserializeObject<T>(data)!;
    }
    
    public static bool ContainsAll(this string text, params string[] containsAll)
    {
        return containsAll.All(text.Contains);
    }
    
    public static bool ContainsAny(this string text, params string[] containsAll)
    {
        return containsAll.Any(text.Contains);
    }

    public static T Random<T>(this IEnumerable<T> arr)
    {
        if (!arr.Any()) return default;
        if (arr.Count() == 1) return arr.First();
        return arr.ElementAt(random.Next(arr.Count()));
    }
    
    public static long GetTimeMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static string GetString(this DiscordUser user) => $"[{user.Id}] | [{user.Username}]";
    public static string GetString(this DiscordGuild server) => $"[{server.Id}] | [{server.Name}]";
}