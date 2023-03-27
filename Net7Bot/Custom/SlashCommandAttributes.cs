using DSharpPlus.SlashCommands;

namespace Net7Bot.Custom;

[AttributeUsage(AttributeTargets.Method)]
public class SlashRequireRealUserAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        return !ctx.User.IsBot;
    }
}