using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Net7Bot.Custom;
using Net7Bot.Other;
using static Net7Bot.Other.CommonMethods;
using static Net7Bot.Program;

namespace Net7Bot.Commands;

[SlashCommandGroup("common", "common/public bot commands")]
public class SlashCommands : ApplicationCommandModule
{
    [SlashCommand("rolldice", "Roll a dice"), SlashRequireRealUser]
    public async Task Rolldice(InteractionContext ctx,
        [Option("sides", "amount of sides the dice should have (max: 100,000)")]
        long sides = 6,
        [Option("times", "amount of times to roll the dice (max: 10,000)")]
        long timesToRoll = 1)
    {
        await ctx.Wait();
        await ctx.UserCheck();
        var sidesInt = (int) Math.Clamp(sides, 1, 100000);
        var timesToRollClamp = (int) Math.Clamp(timesToRoll, 1, 10000);

        List<long> rolled = new();
        for (var i = 0; i < timesToRollClamp; i++) rolled.Add(random.Next(sidesInt) + 1);

        var result = timesToRollClamp < 11
            ? string.Join("\n", rolled)
            : $"""
        Average = [{(float) rolled.Sum() / rolled.Count:###,###}]

        Min = [{rolled.Min():###,###}]
        Mid = [{rolled.Order().ElementAt(rolled.Count / 2):###,###}]
        Max = [{rolled.Max():###,###}]
        Total = [{rolled.Sum()}]
        """;

        await ctx.WaitRespond(new DiscordEmbedBuilder().WithColor(DiscordColor.Teal).WithDescription(result),
            $"You rolled a [{sidesInt:###,###}] sided dice [{timesToRollClamp:###,###}] times\n result:");
    }

    [SlashCommand("ping", "check the bot's response time"), SlashRequireRealUser]
    public async Task Ping(InteractionContext ctx)
    {
        await ctx.Wait();
        await ctx.UserCheck();
        await ctx.WaitRespond($":ping_pong: at `{bot.Ping}ms`");
    }

    [SlashCommand("define", "defines a specified word"), Cooldown(1, 90, CooldownBucketType.Guild),
     SlashRequireRealUser]
    public async Task Definition(InteractionContext ctx,
        [Option("word", "word to define")] string word,
        [Option("user", "user to correct")] DiscordUser user = null)
    {
        await ctx.Wait();
        await ctx.UserCheck();
        if (word.Contains(' '))
        {
            await ctx.WaitRespond("the word given can not contain space");
            return;
        }

        var webCall = await $"https://api.dictionaryapi.dev/api/v2/entries/en/{word}".LoadFromWeb();

        if (webCall is null || webCall.Contains("No Definitions Found"))
        {
            await ctx.WaitRespond("the given word does not exist in my dictionary");
            return;
        }

        var data = (await webCall.LoadJsonFromString<dynamic>())[0];
        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Teal)
            .WithTitle($"**The definition of [{data.word}] is**");

        foreach (var meaning in data.meanings)
        {
            var partOfSpeech = meaning.partOfSpeech.ToString();

            foreach (var definition in meaning.definitions)
            {
                embed.AddField(definition.definition.ToString(), partOfSpeech);
            }
        }

        var content = user is null
            ? "Here is your definition"
            : $"{user.Mention} you need to buy a dictionary because the definition of [{word}] is";

        await ctx.WaitRespond(embed, content);
    }

    [SlashCommand("userstats", "checks the stats of a user"), SlashRequireRealUser]
    public async Task GetUserdata(InteractionContext ctx,
        [Option("user", "user to check, default is yourself")]
        DiscordUser? user = null)
    {
        var discordUser = user ?? ctx.User;

        if (discordUser.IsBot)
        {
            await ctx.CreateResponseAsync("other bots are trash, I don't even want to think of 'em");
            return;
        }

        await ctx.Wait();
        await ctx.UserCheck();
        await ctx.WaitRespond(await discordUser.MakeUserEmbed());
    }

    [SlashCommand("uptime", "how long this instance of the bot has been online"), SlashRequireRealUser]
    public async Task Uptime(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync($"I have been awake for [{(DateTime.Now - start).GetTime()}]");
    }

    [SlashCommand("requestfeature", "request a feature for the bot (can only be used once every 30min/user)"),
     SlashCooldown(1, 1800d, SlashCooldownBucketType.User), SlashRequireRealUser, SlashRequireGuild]
    public async Task RequestFeature(InteractionContext ctx,
        [Option("request", "the feature you want to request")]
        string request)
    {
        await ctx.Wait();

        if (!ctx.Guild.Members.ContainsKey(OwnerId))
        {
            await ctx.WaitRespond(ErrorEmbed("The bot owner, [<@263138275932241920>] is not in this server",
                "User not in server"));
            return;
        }

        var owner = ctx.Guild.Members[OwnerId];

        await owner.SendMessageAsync(new DiscordEmbedBuilder().WithColor(DiscordColor.Teal)
            .WithAuthor("Feature Request").WithDescription(request)
            .WithTitle($"From [{ctx.Guild}]")
            .WithFooter($"From {ctx.User}", ctx.User.AvatarUrl));

        await ctx.WaitRespond("Request sent");
    }

    [SlashCommand("info", "simple information about the bot"), SlashRequireRealUser]
    public async Task Info(InteractionContext ctx)
    {
        await ctx.Wait();
        var embed = new DiscordEmbedBuilder().WithColor(DiscordColor.Teal)
            .WithThumbnail(bot.CurrentUser.AvatarUrl).WithTitle("About Me")
            .WithFooter("Want a feature? do /requestfeature to request a feature!",
                "https://cdn.discordapp.com/attachments/551608556617596938/1053516552433057863/Untitled.png");

        embed.AddField("Creator", "<@263138275932241920>")
            .AddField("Servers ~~infected~~ joined", $"{bot.Guilds.Count}")
            .AddField("Made with", "Main language: C#, using sqlite (sqlite-net-pcl + SQLiteNetExtensions + Newtonsoft.Json) for databasing")
            .AddField("Bot Api ", "DSharpPlus (+ CommandsNext, Interactivity, SlashCommands) v.4.3.0-nightly-01215")
            .AddField("Uptime", $"{(DateTime.Now - start).GetTime()}")
            .AddField("Ping", $"{bot.Ping}");

        await ctx.WaitRespond(embed);
    }
}