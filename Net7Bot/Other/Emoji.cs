using DSharpPlus.Entities;

namespace Net7Bot.Other;

public static class Emoji
{
    public static readonly DiscordEmoji Star = DiscordEmoji.FromName(Program.bot, ":star:");
    public static readonly DiscordEmoji Goodnight = DiscordEmoji.FromGuildEmote(Program.bot, 1052695983391248495);
    public static readonly DiscordEmoji Happy = DiscordEmoji.FromGuildEmote(Program.bot, 1052702758198849596);
    public static readonly DiscordEmoji Sad = DiscordEmoji.FromGuildEmote(Program.bot, 1052702756823105546);
    public static readonly DiscordEmoji Wave = DiscordEmoji.FromGuildEmote(Program.bot, 1053377818790215781);
}