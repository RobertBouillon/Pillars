using System;
using System.Collections.Generic;
namespace System.Modules.v1_0
{
  public interface ILogController
  {
    ILog GetLog(string fullName);
    IMeter FindMetric(string fullName);
    bool IsLogProcessorRunning { get; }

    event EventHandler<ILogEventArgs> LogAdded;
    event EventHandler<ILogEntryEventArgs> LogEntryAdded;

    void ProcessLogs();
    void StartLogProcessor();
    void StopLogProcessor();
    void FlushWatchers();

    IList<ILog> AllLogs { get; }
    IList<ILogWatcher> GlobalWatchers { get; }
    IList<ILogWatcher> AllWatchers { get; }
  }
}
