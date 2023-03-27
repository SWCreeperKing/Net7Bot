using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Net7Bot.Sqlite;

namespace Net7Bot.Events;

public static class ServerUserEvent
{
    public static async Task MainServerUserAdded(DiscordClient bot, GuildMemberAddEventArgs args)
    {
        var server = await args.Guild.GetServer();
        var settings = server.Settings;

        if (!settings.ShowJoinLeaveMessages) return;
        if (!args.Guild.Channels.ContainsKey(settings.WelcomeChannelId)) return;
        var channel = args.Guild.Channels[settings.WelcomeChannelId];
        
        await channel.SendMessageAsync(await MakeUserEmbed(settings.JoinMessage, args.Member, args.Guild.Name));
    }

    public static async Task MainServerUserRemoved(DiscordClient bot, GuildMemberRemoveEventArgs args)
    {
        var server = await args.Guild.GetServer();
        var settings = server.Settings;

        if (!settings.ShowJoinLeaveMessages) return;
        if (!args.Guild.Channels.ContainsKey(settings.WelcomeChannelId)) return;
        var channel = args.Guild.Channels[settings.WelcomeChannelId];

        await channel.SendMessageAsync(await MakeUserEmbed(settings.LeaveMessage, args.Member, args.Guild.Name));
    }

    public static async Task<DiscordEmbedBuilder> MakeUserEmbed(string joinLeaveMessage, DiscordMember user,
        string serverName)
    {
        var message = joinLeaveMessage;

        message = message.Replace("[user]", $"[{user.Username}#{user.Discriminator}]")
            .Replace("[server]", serverName);

        return new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Teal)
            .WithTitle("Welcome user!")
            .WithDescription(message)
            .WithFooter($"user Id: {user.Id}]");
    }
}