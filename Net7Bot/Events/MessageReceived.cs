using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Net7Bot.Other;
using Net7Bot.Sqlite;
using static Net7Bot.Other.HelperFunctions;

namespace Net7Bot.Events;

public class MessageReceived
{
    public static readonly Regex HruRegex = new(@"^(how|h)( are|( |)r)( you|( |)u)", RegexOptions.Compiled);
    public static readonly Regex GoodBotRegex = new(@"^good bot", RegexOptions.Compiled);
    public static readonly Regex SadBotRegex = new(@"^bad bot", RegexOptions.Compiled);

    public static readonly Regex GreetingsMessages =
        new(@"^good( |)(morning|evening|afternoon)", RegexOptions.Compiled);

    public static readonly Regex Goodnight = new(@"^good( |)night", RegexOptions.Compiled);

    public static string[] HruMessages = { "good", "fine", "ok", "could be better", "pretty good so far", "not bad" };

    public static async Task MainMessageCreatedEvent(DiscordClient bot, MessageCreateEventArgs rawMessage)
    {
        var message = rawMessage.Message;
        var server = rawMessage.Guild;
        if (server is null) return;

        var user = message.Author;
        if (user.IsBot) return;

        var botUser = await server.GetMemberAsync(bot.CurrentUser.Id);
        var userData = await user.GetUser();
        var serverData = await server.GetServer();

        if (await serverData.CheckAddUser(userData) && serverData.Settings.PassiveInteraction &&
            botUser.Permissions.HasPermission(Permissions.AddReactions))
        {
            await message.CreateReactionAsync(Emoji.Wave);
        }

        if (serverData.Settings.PassiveInteraction)
        {
            var rawContent = message.Content;
            var contentLow = rawContent.ToLower();
            if (message.MentionedUsers.Contains(botUser))
            {
                if (HruRegex.IsMatch(contentLow))
                {
                    await message.RespondAsync(HruMessages.Random());
                }
                else if (contentLow.ContainsAny("fuck you"))
                {
                    await message.CreateReactionAsync(Emoji.Sad);
                }
            }
            else
            {
                var now = GetTimeMs();
                if (Goodnight.IsMatch(contentLow))
                {
                    await message.CreateReactionAsync(Emoji.Goodnight);
                }
                else if (GreetingsMessages.IsMatch(contentLow) && now - serverData.Cache.LastGoodGreeting >= 36e6)
                {
                    serverData.Cache.LastGoodGreeting = now;
                    var realNow = DateTime.Now;
                    var greet = realNow.Hour switch
                    {
                        > 4 and < 12 => "Good morning!",
                        >= 12 and < 20 => "Good afternoon!",
                        _ => "Good evening!"
                    };
                    await message.RespondAsync($"{greet} It is currently [{realNow:t}]");
                }
                else if (GoodBotRegex.IsMatch(contentLow))
                {
                    await message.CreateReactionAsync(Emoji.Happy);
                }
                else if (SadBotRegex.IsMatch(contentLow))
                {
                    await message.CreateReactionAsync(Emoji.Sad);
                }
            }
        }

        userData.TotalMessageCount++;
        if (userData.Username != user.Username) userData.Username = user.Username;

        await userData.UpdateTable();
        await serverData.UpdateTable();
    }
}