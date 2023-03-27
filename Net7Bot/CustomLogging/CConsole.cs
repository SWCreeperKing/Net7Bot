using Net7Bot.Sqlite;
using static System.ConsoleKey;
using static Net7Bot.Program;

namespace Net7Bot.CustomLogging;

public static class CConsole
{
    public static readonly char[] numberSet = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
    public static readonly Queue<Log> logs = new();
    public static string input = "";
    public static int? lastLine = null;

    public static async Task RunConsole()
    {
        while (true)
        {
            Console.CursorVisible = false;
            while (!Console.KeyAvailable && input.Length is 0)
            {
                if (!logs.Any())
                {
                    await Task.Delay(100);
                    continue;
                }

                var message = logs.Dequeue();
                Console.ForegroundColor = message.level.Color();
                WriteLine($"[{DateTime.Now:g}] {message.message}");
                Console.ForegroundColor = ConsoleColor.White;

                await Task.Delay(50);
            }

            var key = Console.ReadKey(true);
            if (input != "") Console.CursorTop = Console.GetCursorPosition().Top - 1;
            switch (key.Key)
            {
                case Backspace:
                    input = input.Remove(input.Length - 1);
                    break;
                case Tab:
                    input += "   ";
                    break;
                case Spacebar:
                    input += " ";
                    break;
                case Multiply:
                    input += "*";
                    break;
                case Add:
                    input += "+";
                    break;
                case Subtract:
                    input += "-";
                    break;
                case ConsoleKey.Decimal:
                    input += ".";
                    break;
                case Divide:
                    input += "/";
                    break;
                case Enter:
                    WriteLine($"Your input: [{input}]  ");
                    RunCommand(input);
                    input = "";
                    continue;
                default:
                    var i = (int) key.Key;
                    switch (i)
                    {
                        case >= 48 and <= 57:
                            var shift = (key.Modifiers & ConsoleModifiers.Shift) != 0;
                            var num = (i - 48) % 10;
                            if (shift) input += numberSet[num];
                            else input += $"{num}";
                            break;
                        case >= 65 and <= 90:
                            var added = (key.Modifiers & ConsoleModifiers.Shift) != 0 ? 0 : 32;
                            input += $"{(char) (i + added)}";
                            break;
                    }

                    break;
            }

            if (input != "") WriteLine($"Your input: [{input}]  ");
            else DeleteLastLine();
        }
    }

    public static async Task RunCommand(string input)
    {
        DeleteLastLine();
        switch (input.ToLower())
        {
            case "servers":
                var serverList = bot.Guilds.Values.ToArray();
                var serverNames = serverList.Select(s =>
                {
                    var name = s.Name;
                    if (name.Length > 55) name = $"{s.Name[..55]}[...]";
                    return name;
                }).ToArray();
                var serverAmount = serverNames.Length;

                var maxName = serverNames.Max(s => s.Length) + 2;
                var numLength = $"{serverAmount}".Length;
                for (var i = 0; i < serverAmount; i++)
                {
                    Console.BackgroundColor = (i + 1) % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Black;
                    var numberString = $"{i + 1:###,##0}";
                    var owner = serverList[i].Owner;
                    if (owner.Id == OwnerId)
                    {
                        Console.WriteLine("[private]");
                        continue;
                    }

                    Console.Write($"{numberString.PadLeft(numLength)}  ");
                    Console.Write($"{serverNames[i]}".PadRight(maxName));
                    Console.WriteLine($"{owner.Username}#{owner.Discriminator}   ({owner.Id})");
                }

                Console.BackgroundColor = ConsoleColor.Black;
                break;
            case "exit":
                await bot.DisconnectAsync();
                bot.Dispose();
                await SqlSetup.databaseConnection.CloseAsync();
                Environment.Exit(0);
                break;
        }

        Console.WriteLine($"RAN: [{input}]");
    }

    public static void DeleteLastLine()
    {
        if (lastLine is null) return;
        Console.CursorTop = Console.GetCursorPosition().Top - 1;
        Console.WriteLine(string.Join("", Enumerable.Repeat(' ', lastLine.Value)));
        Console.CursorTop = Console.GetCursorPosition().Top - 1;
        lastLine = null;
    }

    public static void WriteLine(string line)
    {
        Console.WriteLine(line);
        lastLine = line.Length;
    }
}

public readonly struct Log
{
    public readonly Logger.Level level;
    public readonly string message;

    public Log(Logger.Level level, string message)
    {
        this.level = level;
        this.message = message;
    }
}