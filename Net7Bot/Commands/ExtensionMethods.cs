using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Net7Bot.Sqlite;
using static Net7Bot.Other.CommonMethods;

namespace Net7Bot.Commands;

public static class ExtensionMethods
{
    public static async Task UserCheck(this InteractionContext ctx)
    {
        var user = await ctx.User.GetUser();
        var mentionedUsers = ctx.ResolvedUserMentions;

        var isDms = ctx.IsDm();
        var isMentionedExist = mentionedUsers is not null && mentionedUsers.Any();

        var server = !isDms ? await ctx.Guild!.GetServer() : null;
        if (!isDms) await server!.CheckAddUser(user);
        if (!isMentionedExist) return;

        foreach (var mentionedUser in mentionedUsers!)
        {
            var currUser = await mentionedUser.GetUser();
            if (!isDms) await server!.CheckAddUser(currUser);
        }
    }

    public static async Task UserCheck(this ContextMenuContext ctx)
    {
        var user = await ctx.User.GetUser();
        var targetedUser = await ctx.GetDiscordUser().GetUser();
        if (ctx.IsDm()) return;
        var server = await ctx.Guild!.GetServer();
        await server.CheckAddUser(user);
        await server.CheckAddUser(targetedUser);
    }

    public static DiscordUser GetDiscordUser(this ContextMenuContext ctx)
    {
        return ctx.TargetUser ?? ctx.TargetMessage.Author;
    }

    public static bool IsDm(this BaseContext ctx) => ctx.Guild is null;

    public static async Task Wait(this BaseContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
    }

    public static async Task WaitRespond(this BaseContext ctx, DiscordWebhookBuilder builder)
    {
        await ctx.EditResponseAsync(builder);
    }

    public static async Task WaitRespond(this BaseContext ctx, string message)
    {
        await ctx.WaitRespond(new DiscordWebhookBuilder().WithContent(message));
    }

    public static async Task WaitRespond(this BaseContext ctx, DiscordEmbedBuilder builder, string message = "")
    {
        await ctx.WaitRespond(new DiscordWebhookBuilder().AddEmbed(builder).WithContent(message));
    }

    public static async Task WaitRespond(this BaseContext ctx, Exception e)
    {
        await ctx.WaitRespond(ErrorEmbed(e));
    }

    public static async Task CreateError(this BaseContext ctx, Exception e)
    {
        await ctx.CreateResponseAsync(ErrorEmbed(e));
    }

    public static string GetTime(this TimeSpan time)
    {
        StringBuilder sb = new();
        if (time.TotalDays >= 1) sb.Append($"{Math.Floor(time.TotalDays):###,##0}d ");
        if (time.Hours > 0) sb.Append($"{time.Hours}h ");
        if (time.Minutes > 0) sb.Append($"{time.Minutes}min ");
        if (time.Seconds > 0) sb.Append($"{time.Seconds}sec ");
        if (time.Milliseconds > 0) sb.Append($"{time.Milliseconds}ms ");

        var ns = (time.Nanoseconds + time.Microseconds * 1000f) / 100 / 10;
        if (ns > 0) sb.Append($"{ns:##0.#}µs");
        return sb.ToString().TrimEnd();
    }
}