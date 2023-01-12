using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;

namespace Spin.Pillars.Logging;

public interface ILogger
{
  LogEntry Capture(string message, Action action, params object[] data);
  LogEntry Catch(string message, Action action, params object[] data);
  LogEntry Error(params object[] data);
  LogScope StartOperation(string operationName, params object[] data);
  string ToString();
  LogEntry Write(Exception ex);
  LogEntry Write(IEnumerable<object> data);
  LogEntry Write(params object[] data);
  LogEntry Write(string message, IEnumerable<object> data);
  LogEntry Write(string message, params object[] data);
}