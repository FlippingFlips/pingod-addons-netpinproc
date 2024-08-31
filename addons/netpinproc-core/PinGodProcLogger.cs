using Godot;
using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using PinGod.Core;

/// <summary> Wraps Godot.Print logging which is used when a PROC game is created</summary>
internal class PinGodProcLogger : ILoggerPROC
{
    public LogLevel LogLevel { get; set; }
    public string LogPrefix { get; set; } = "[PG-PROC]";
    public bool TimeStamp { get; set; } = true;

    public void Log(string text)
    {
        if (CanLog(LogLevel.Info))
            Logger.Info("P-ROC:", text);
    }

    public void Log(string text, LogLevel logLevel = LogLevel.Info)
    {
        if (CanLog(logLevel))
        {
            GD.Print(text);
        }
    }

    public void Log(LogLevel logLevel = LogLevel.Info, params object[] logObjs)
    {
        if (CanLog(logLevel))
        {
            GD.Print(logObjs);
        }
    }

    public void Log(params object[] logObjs)
    {
        GD.Print(logObjs);
    }

    bool CanLog(LogLevel logLevel) => LogLevel <= logLevel;
}
