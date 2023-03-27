using DSharpPlus.Entities;
using Net7Bot.Sqlite;

namespace Net7Bot.Other;

public static class CommonMethods
{
    public static async Task<DiscordEmbedBuilder> MakeUserEmbed(this DiscordUser discordUser)
    {
        var userData = await discordUser.GetUser();
        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Teal)
            .WithThumbnail(discordUser.AvatarUrl)
            .WithTitle($"{discordUser.Username}'s stats")
            .WithFooter($"{discordUser.Id}");

        embed.AddField("Total messages sent", $"{userData.TotalMessageCount:###,##0}");

        return embed;
    }
    
    public static DiscordEmbedBuilder ErrorEmbed(string error = "Unknown Error",
        string titleMessage = "An internal error occured, please contact developer")
    {
        return new DiscordEmbedBuilder().WithColor(DiscordColor.Red).WithTitle(titleMessage).WithDescription(error);
    }

    public static DiscordEmbedBuilder ErrorEmbed(Exception e)
    {
        var errorString = e.ToString();
        return new DiscordEmbedBuilder().WithColor(DiscordColor.Red)
            .WithTitle("An internal error occured, please contact developer")
            .WithDescription(errorString[..Math.Min(errorString.Length, 2000)]);
    }
}