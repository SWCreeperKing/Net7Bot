using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using Net7Bot.Commands;
using Net7Bot.Custom;
using Net7Bot.CustomLogging;
using Serilog;
using static Net7Bot.Events.MessageReceived;
using static Net7Bot.Events.ServerUserEvent;
using static Net7Bot.Other.CommonMethods;
using static Net7Bot.Sqlite.SqlSetup;
using Log = Serilog.Log;

namespace Net7Bot;

public static class Program
{
    public const ulong OwnerId = 263138275932241920;

    public static readonly DateTime start = DateTime.Now;
    public static readonly Random random = new();
    public static DiscordClient bot;

    public static void Main(string[] args)
    {
        Task.Run(CConsole.RunConsole);
        InitSqlite();
        MainAsync().GetAwaiter().GetResult();
    }

    public static async Task MainAsync()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Logger()
            .CreateLogger();

        var loggerFactory = new LoggerFactory().AddSerilog();

        bot = new DiscordClient(new DiscordConfiguration
        {
            Token = Token.BotToken, TokenType = TokenType.Bot, Intents = DiscordIntents.All,
            LoggerFactory = loggerFactory
        });

        var slash = bot.UseSlashCommands();
        slash.SlashCommandErrored += async (_, e) => await ErrorHandle(e);
        slash.ContextMenuErrored += async (_, e) => await ErrorHandle(e);

        slash.RegisterCommands<SlashCommands>();
        slash.RegisterCommands<SettingsCommands>();
        slash.RegisterCommands<ContextMenus>();

        bot.MessageCreated += MainMessageCreatedEvent;
        bot.GuildMemberAdded += MainServerUserAdded;
        bot.GuildMemberRemoved += MainServerUserRemoved;

        await bot.ConnectAsync();
        Logger.Log(Logger.Level.Special, "Bot Started");
        await Task.Delay(-1);
    }

    public static async Task ErrorHandle(SlashCommandErrorEventArgs e)
    {
        if (e.Exception is SlashExecutionChecksFailedException sca)
        {
            foreach (var excCheck in sca.FailedChecks)
            {
                switch (excCheck)
                {
                    case SlashRequireUserPermissionsAttribute req:
                        await SendAttributeMessage(e.Context, "Missing Permissions Required",
                            $"**Inorder to execute this command you are missing the following permissions**:\n [{req.Permissions.ToPermissionString()}]");
                        break;
                    case SlashRequireRealUserAttribute:
                        await SendAttributeMessage(e.Context, "You are not real",
                            $"This command can only be executed by **real** people, which you [{e.Context.User}] are not!");
                        break;
                    case SlashRequireGuildAttribute:
                        await SendAttributeMessage(e.Context, "This is not a server",
                            "This command can only be executed in a server");
                        break;
                }
            }
        }
        else await ErrorHandle(e.Context, e.Exception);
    }

    public static async Task ErrorHandle(ContextMenuErrorEventArgs e)
    {
        await ErrorHandle(e.Context, e.Exception);
    }

    public static async Task ErrorHandle(BaseContext context, Exception e)
    {
        await SendHandleMessage(context, new DiscordInteractionResponseBuilder().AddEmbed(ErrorEmbed(e)));
    }

    public static async Task SendAttributeMessage(BaseContext context, string title, string message)
    {
        await SendHandleMessage(context, new DiscordInteractionResponseBuilder()
            .AddEmbed(ErrorEmbed(message, title)));
    }

    public static async Task SendHandleMessage(BaseContext context, DiscordInteractionResponseBuilder builder)
    {
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }
}