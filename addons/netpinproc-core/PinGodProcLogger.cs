using Godot;
using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using PinGod.Core;
using System;

/// <summary> Wraps Godot.Print logging which is used when a PROC game is created</summary>
internal class PinGodProcLogger : ILoggerPROC
{
    public LogLevel LogLevel { get; set; }
    public string LogPrefix { get; set; } = "[PROC]";
    public bool TimeStamp { get; set; } = true;

    /// <summary>log restricted by loglevel info</summary>
    /// <param name="text"></param>
    public void Log(string text)
    {
        if (this.LogLevel <= LogLevel.Info)
            GD.Print($"{text}");
    }

    /// <summary>log restricted by loglevel</summary>
    /// <param name="text"></param>
    public void Log(string text, LogLevel logLevel = LogLevel.Info)
    {
        if (CanLog(logLevel))
        {
            GD.Print($"{GetPrefix(logLevel)}{text}");
        }
    }

    /// <summary>log restricted by loglevel</summary>
    /// <param name="text"></param>
    public void Log(LogLevel logLevel = LogLevel.Info, params object[] logObjs)
    {
        if (CanLog(logLevel))
        {
            var @params = new object[logObjs.Length + 1];
            @params[0] = $"{GetPrefix(logLevel)}";
            logObjs.CopyTo(@params, 1);
            GD.Print(@params);
        }
    }

    /// <summary>Just GD print, no levels</summary>
    /// <param name="logObjs"></param>
    public void Log(params object[] logObjs) => GD.Print(logObjs);

    bool CanLog(LogLevel logLevel) => LogLevel <= logLevel;

    private string GetPrefix(LogLevel logLevel = LogLevel.Info)
    {
        var ts = TimeStamp ? DateTime.Now.TimeOfDay.ToString() : null;
        return $"{LogPrefix}[{logLevel}][{ts}]:";
    }
}
