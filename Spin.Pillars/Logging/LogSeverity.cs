using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spin.Pillars.Logging;

public enum LogSeverity
{
  /// <summary>
  /// A trace used only during run-time debugging. Verbose traces should be disabled during normal operation
  /// </summary>
  Verbose = 0,

  /// <summary>
  /// A normal event has occurred that should be logged.
  /// </summary>
  Trace = 1,

  /// <summary>
  /// An abnormal event has occurred that should be logged, but lacks the severity to notify an administrator
  /// </summary>
  Notification = 2,

  /// <summary>
  /// An abnormal event has occurred and requires administrative attention
  /// </summary>
  Alert = 3,

  /// <summary>
  /// An application error has occurred and requires administrative attention.
  /// </summary>
  Error = 4,

  /// <summary>
  /// A critical system error has occurred and requires immediate administrative attention.
  /// </summary>
  FatalError = 5
}

public static class LogSeverityExtensions
{
  private static Dictionary<LogSeverity, LogLevel> LogLevelMapping = new Dictionary<LogSeverity, LogLevel>()
  {
    {LogSeverity.Verbose, LogLevel.Debug},
    {LogSeverity.Trace, LogLevel.Trace},
    {LogSeverity.Notification, LogLevel.Information},
    {LogSeverity.Alert, LogLevel.Warning},
    {LogSeverity.Error, LogLevel.Error},
    {LogSeverity.FatalError, LogLevel.Critical}
  };

  public static LogLevel ToLogLevel(this LogSeverity severity)
  {
    if (LogLevelMapping.TryGetValue(severity, out var level))
      return level;

    throw new NotImplementedException(severity.ToString());
  }
}
