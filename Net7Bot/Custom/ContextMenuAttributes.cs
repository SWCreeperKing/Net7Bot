using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace Net7Bot.Custom;

public class ContextRequireUserPermissions : ContextMenuCheckBaseAttribute
{
    public Permissions permissionCheck;

    public ContextRequireUserPermissions(Permissions permissionCheck)
    {
        this.permissionCheck = permissionCheck;
    }

    public override async Task<bool> ExecuteChecksAsync(ContextMenuContext ctx)
    {
        return ctx.Member.Permissions.HasPermission(permissionCheck);
    }
}