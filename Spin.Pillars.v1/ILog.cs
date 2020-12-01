using System;
using System.Collections.Generic;
namespace Spin.Pillars.v1
{
  public interface ILog
  {
    IModule Module { get; }

    void Write(ILogEntry log);
    ILogEntry Write(LogSeverity severity, Exception ex, string text, params object[] args);
    ILogEntry Write(LogSeverity severity, string format, params object[] args);
    ILogEntry Write(Exception ex, string text, params object[] args);
    ILogEntry Write(Exception ex);
    ILogEntry Write(string format, params object[] args);
  }
}
