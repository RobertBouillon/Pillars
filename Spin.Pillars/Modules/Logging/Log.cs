﻿using Microsoft.Extensions.Logging;
using Spin.Pillars.Modules.Logging.Writers;
using Spin.Pillars.Time;
using System;
using System.Linq;

namespace Spin.Pillars.Logging
{
  public class Log
  {
    #region Properties
    public LogWriter Writer { get; }
    public IClock Clock { get; }
    public Module Module { get; set; }
    #endregion
    #region Constructors
    internal Log(Module module, LogWriter writer, IClock clock)
    {
      #region Validation
      if (writer is null)
        throw new ArgumentNullException(nameof(writer));
      if (clock is null)
        throw new ArgumentNullException(nameof(clock));
      if (Module is null)
        throw new ArgumentNullException(nameof(Module));
      #endregion
      Writer = writer;
      Clock = clock;
      Module = module;
    }
    #endregion
    #region Methods
    public LogEntry Write(string format, params object[] args) => Write(LogSeverity.Trace, format, args);
    public LogEntry Write(Exception ex, string text, params object[] args) => Write(LogSeverity.Error, ex, text, args);
    public LogEntry Write(Exception ex) => Write(ex, ex.Message);

    public void Write(LogEntry log) => Writer.Write(log);

    public LogEntry Write(LogSeverity severity, string format, params object[] args)
    {
      var ret = new LogEntry
      {
        EntryTime = Clock.Time,
        Module = Module,
        FormatText = format,
        Arguments = args,
        Severity = severity
      };

      Write(ret);
      return ret;
    }

    public LogEntry Write(LogSeverity severity, Exception ex, string text, params object[] args)
    {
      var ret = new LogEntry
      {
        EntryTime = Clock.Time,
        FormatText = text,
        Arguments = args,
        Severity = severity,
        Error = new ExceptionInfo(ex),
        Module = Module
      };

      Write(ret);
      return ret;
    }
    #endregion

    #region Overrides
    public override string ToString() => Module.Path.ToString('\\');
    #endregion
  }
}
