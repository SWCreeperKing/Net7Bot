using DSharpPlus.SlashCommands;
using Net7Bot.Other;
using static DSharpPlus.ApplicationCommandType;

namespace Net7Bot.Commands;

public class ContextMenus : ApplicationCommandModule
{
    [ContextMenu(UserContextMenu, "Get User Data")]
    public async Task UserDataMenu(ContextMenuContext ctx)
    {
        await ctx.Wait();
        var user = ctx.GetDiscordUser();
        if (user.IsBot || ctx.User.IsBot)
        {
            await ctx.WaitRespond("I don't like looking at other bots");
            return;
        }

        await ctx.UserCheck();
        await ctx.WaitRespond(await user.MakeUserEmbed());
    }

    [ContextMenu(MessageContextMenu, "Get User Data_")]
    public async Task UserDataMenuMessage(ContextMenuContext ctx) => await UserDataMenu(ctx);
}