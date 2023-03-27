using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using static Net7Bot.CustomLogging.Logger.Level;

namespace Net7Bot.CustomLogging;

public static class Logger
{
    public enum Level
    {
        Info,
        Debug,
        Special,
        Warning,
        SoftError,
        Error,
        Other
    }

    public static readonly string Guid = System.Guid.NewGuid().ToString();

    public static bool showDebugLogs = true;

    private static IList<Action<Level, string>> _listeners = new List<Action<Level, string>>();
    private static IList<string> _log = new List<string>();

    public static event Action<Level, string> LoggerListeners
    {
        add => _listeners.Add(value);
        remove => _listeners.Remove(value);
    }

    public static void Log(object level, string text) => Log(Debug, text);
    public static void Log(object text) => Log(Debug, text.ToString());

    public static void Log(Exception e, string furtherInfo = "")
    {
        if (furtherInfo is not "") Log(Warning, $"Error Info: {furtherInfo}");
        Log(Error, e.ToString());
    }

    public static T LogReturn<T>(T t)
    {
        Log(t.ToString());
        return t;
    }

    public static void Log(Level level, string text)
    {
        if (!showDebugLogs && level is Debug) return;
        CConsole.logs.Enqueue(new Log(level, text));
    }

    public static void Log(Level level, object text) => Log(level, text.ToString());
}

public class LoggerSink : ILogEventSink
{
    private readonly IFormatProvider _formatProvider;
    public LoggerSink(IFormatProvider formatProvider) => _formatProvider = formatProvider;

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage(_formatProvider);
        if (message is null
            || message.Contains("DSharpPlus, version")
            || message.Contains("Connection terminated")
            || message.Contains("Session resumed")
            || message.Contains("Unknown event")
            // || message.Contains("Connection attempt failed")
            )
        {
            return;
        }

        Logger.Log(logEvent.Level switch
        {
            LogEventLevel.Debug => Debug,
            LogEventLevel.Verbose => Special,
            LogEventLevel.Information => Info,
            LogEventLevel.Warning => Warning,
            LogEventLevel.Error => SoftError,
            LogEventLevel.Fatal => Error,
            _ => Logger.Level.Other
        }, $"From DSP: {message}");
    }
}

public static class LoggerExtensions
{
    public static LoggerConfiguration Logger(this LoggerSinkConfiguration loggerConfiguration,
        IFormatProvider formatProvider = null)
    {
        return loggerConfiguration.Sink(new LoggerSink(formatProvider));
    }

    public static ConsoleColor Color(this Logger.Level level)
    {
        return level switch
        {
            Info => ConsoleColor.DarkGreen,
            Debug => ConsoleColor.DarkCyan,
            Warning => ConsoleColor.Yellow,
            Error or SoftError => ConsoleColor.Red,
            CustomLogging.Logger.Level.Other => ConsoleColor.Blue,
            Special => ConsoleColor.Cyan,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}