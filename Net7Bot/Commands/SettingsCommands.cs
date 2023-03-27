using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Net7Bot.Custom;
using Net7Bot.Sqlite;

namespace Net7Bot.Commands;

[SlashCommandGroup("settings", "bot settings")]
public class SettingsCommands : ApplicationCommandModule
{
    [SlashCommand("passiveinteractivity", "if the bot can interact with members passively (without commands)"),
     SlashRequireUserPermissions(Permissions.Administrator), SlashRequireRealUser, SlashRequireGuild]
    public async Task PassiveInteractivity(InteractionContext ctx,
        [Option("value", "value to set the passive interactivity to (null to get)")]
        bool? set = null)
    {
        await ctx.Wait();
        await ctx.UserCheck();

        if (ctx.IsDm())
        {
            await ctx.WaitRespond("This is not a valid server");
            return;
        }

        var server = await ctx.Guild.GetServer();

        if (set is not null)
        {
            server.Settings.PassiveInteraction = set.Value;
            await server.UpdateTable();
        }

        await ctx.WaitRespond(
            $"passive interactivity is{
                (set is null ? "" : " now")} {
                    (server.Settings.PassiveInteraction ? "on" : "off")}");
    }

    [SlashCommand("welcomemessages", "sets the settings for the join/leave messages"),
     SlashRequireUserPermissions(Permissions.Administrator), SlashRequireRealUser, SlashRequireGuild]
    public async Task WelcomeMessages(InteractionContext ctx,
        [Option("toggle", "turn it off or on")]
        bool? toggle = null,
        [Option("channel", "channel to put the join/leave messages in")]
        DiscordChannel? channel = null,
        [Option("joinmsg", "message to display when someone joins, keys: [user], [server]")]
        string? joinMessage = null,
        [Option("leavemsg", "message to display when someone leaves, keys: [user], [server]")]
        string? leaveMessage = null)
    {
        await ctx.Wait();

        var serverRaw = ctx.Guild;
        var server = await serverRaw.GetServer();
        var settings = server.Settings;

        if (toggle is not null) settings.ShowJoinLeaveMessages = toggle.Value;
        if (channel is not null) settings.WelcomeChannelId = channel.Id;
        if (joinMessage is not null) settings.JoinMessage = joinMessage;
        if (leaveMessage is not null) settings.LeaveMessage = leaveMessage;

        await server.UpdateTable();

        var channelString = serverRaw.Channels.Any(kv => kv.Key == settings.WelcomeChannelId)
            ? serverRaw.Channels[settings.WelcomeChannelId].Mention
            : "N/A";

        await ctx.WaitRespond(new DiscordEmbedBuilder().WithColor(DiscordColor.Teal)
            .AddField("Is enabled?", settings.ShowJoinLeaveMessages ? "yes" : "no")
            .AddField("Welcome channel", channelString)
            .AddField("Join message", settings.JoinMessage)
            .AddField("Leave message", settings.LeaveMessage));
    }
}