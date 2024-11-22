using NLog;
using NLog.Config;
using NLog.Targets;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Playnite;

public interface ILogger
{
    void Info(string message);
    void Info(object message);
    void Debug(string message);
    void Debug(object message);
    void Debug(Exception exception, string message);
    void Debug(Exception exception, object message);
    void Warn(string message);
    void Warn(object message);
    void Warn(Exception exception, string message);
    void Warn(Exception exception, object message);
    void Error(string message);
    void Error(object message);
    void Error(Exception exception, string message);
    void Error(Exception exception, object message);
    void Trace(string message);
    void Trace(object message);
    void Trace(Exception exception, string message);
    void Trace(Exception exception, object message);
}

public class NullLoggger : ILogger
{
    public void Debug(string message)
    {
    }

    public void Debug(object message)
    {
    }

    public void Debug(Exception exception, string message)
    {
    }

    public void Debug(Exception exception, object message)
    {
    }

    public void Error(string message)
    {
    }

    public void Error(object message)
    {
    }

    public void Error(Exception exception, string message)
    {
    }

    public void Error(Exception exception, object message)
    {
    }

    public void Info(string message)
    {
    }

    public void Info(object message)
    {
    }

    public void Trace(string message)
    {
    }

    public void Trace(object message)
    {
    }

    public void Trace(Exception exception, string message)
    {
    }

    public void Trace(Exception exception, object message)
    {
    }

    public void Warn(string message)
    {
    }

    public void Warn(object message)
    {
    }

    public void Warn(Exception exception, string message)
    {
    }

    public void Warn(Exception exception, object message)
    {
    }
}

public interface ILogProvider
{
    ILogger GetLogger();
    ILogger GetLogger(string loggerName);
}

public class LogManager
{
    private static readonly NullLoggger nullLogger = new();
    private static ILogProvider? logManager;

    internal static void SetLogManager(ILogProvider manager)
    {
        logManager = manager;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ILogger GetLogger()
    {
        return logManager?.GetLogger() ?? nullLogger;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ILogger GetLogger(string loggerName)
    {
        return logManager?.GetLogger(loggerName) ?? nullLogger;
    }
}

public class NLogLogger : ILogger
{
    private readonly NLog.Logger logger;

    public NLogLogger(NLog.Logger logger)
    {
        this.logger = logger;
    }

    public void Debug(string message)
    {
        logger.Debug(message);
    }

    public void Debug(Exception exception, string message)
    {
        logger.Debug(exception, message);
    }

    public void Error(string message)
    {
        logger.Error(message);
    }

    public void Error(Exception exception, string message)
    {
        logger.Error(exception, message);
    }

    public void Info(string message)
    {
        logger.Info(message);
    }

    public void Info(Exception exception, string message)
    {
        logger.Info(exception, message);
    }

    public void Warn(string message)
    {
        logger.Warn(message);
    }

    public void Warn(Exception exception, string message)
    {
        logger.Warn(exception, message);
    }

    public void Trace(string message)
    {
        if (NLogLogProvider.TraceLoggingEnabled)
        {
            logger.Trace(message);
        }
    }

    public void Trace(Exception exception, string message)
    {
        if (NLogLogProvider.TraceLoggingEnabled)
        {
            logger.Trace(exception, message);
        }
    }

    public void Info(object message)
    {
        Info(message.ToString() ?? "null");
    }

    public void Debug(object message)
    {
        Debug(message.ToString() ?? "null");
    }

    public void Debug(Exception exception, object message)
    {
        Debug(exception, message.ToString() ?? "null");
    }

    public void Warn(object message)
    {
        Warn(message.ToString() ?? "null");
    }

    public void Warn(Exception exception, object message)
    {
        Warn(exception, message.ToString() ?? "null");
    }

    public void Error(object message)
    {
        Error(message.ToString() ?? "null");
    }

    public void Error(Exception exception, object message)
    {
        Error(exception, message.ToString() ?? "null");
    }

    public void Trace(object message)
    {
        Trace(message.ToString() ?? "null");
    }

    public void Trace(Exception exception, object message)
    {
        Trace(exception, message.ToString() ?? "null");
    }
}

public class NLogLogProvider : ILogProvider
{
    private static readonly Dictionary<string, LogFactory> factories = new(StringComparer.OrdinalIgnoreCase);
    public static bool TraceLoggingEnabled { get; set; } = false;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public ILogger GetLogger()
    {
        // StackFrame 2 because this is always used via log manager SDK class.
        var callingMethod = new StackFrame(2).GetMethod();
        var typeName = callingMethod?.DeclaringType?.Name ?? "uknown";
        return GetLogger(typeName, PlaynitePaths.LogFile);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public ILogger GetLogger(string loggerName)
    {
        // StackFrame 2 because this is always used via log manager SDK class.
        return GetLogger(loggerName, PlaynitePaths.LogFile);
    }

    public static ILogger GetLogger(string loggerName, string loggerFile)
    {
        if (factories.TryGetValue(loggerFile, out var logFactory))
        {
            return new NLogLogger(logFactory.GetLogger(loggerName));
        }

        var config = new LoggingConfiguration();
        if (!loggerFile.IsNullOrWhiteSpace())
        {
            var fileTarget = new FileTarget
            {
                Name = loggerName,
                FileName = loggerFile,
                Layout = @"${date:format=dd-MM HH\:mm\:ss.fff}|${level:uppercase=true:padding=-5}|${logger}:${message}${onexception:${newline}${exception:format=toString}}",
                KeepFileOpen = true,
                ArchiveFileName = Path.ChangeExtension(loggerFile, ".{#####}.log"),
                ArchiveAboveSize = 4_096_000,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                MaxArchiveFiles = 2,
                Encoding = Encoding.UTF8
            };

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
        }

#if DEBUG
        var debugTarget = new CustomDebugLogTarget
        {
            Name = "DebuggerOutput",
            Layout = @"${level:uppercase=true:padding=-5}|${logger}:${message}${onexception:${newline}${exception}}"
        };

        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, debugTarget));
#endif

        logFactory = new LogFactory();
        logFactory.Configuration = config;
        factories.Add(loggerFile, logFactory);
        return new NLogLogger(logFactory.GetLogger(loggerName));
    }
}

// Used because NLog internally uses Debugger.Log which results in additional string including "level" and "category"
// being appended at the start of each message in debug output in Rider. Couldn't find any option in Rider to disable
// this so therefore custom logger. Visual Studio doesn't add level and category, but there's also no option to show them
// if you needed them so this works well with both VS and Rider.
[Target("CustomDebugLog")]
public class CustomDebugLogTarget : TargetWithLayout
{
    protected override void Write(LogEventInfo logEvent)
    {
        var logMessage = RenderLogEvent(this.Layout, logEvent);
        Debug.WriteLine(logMessage);
    }
}