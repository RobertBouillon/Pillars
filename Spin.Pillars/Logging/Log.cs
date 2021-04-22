using Spin.Pillars.Logging.Data;
using Spin.Pillars.Logging.Readers;
using Spin.Pillars.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.Logging
{
  public static class Log
  {
    public static IClock Clock { get; } = new Clock();

    public static object ErrorData => new Label("Error");
    public static string StartingOperationStatus => "Started";
    public static string FinishedOperationStatus => "Finished";

    public static Action<LogEntry> WriteHandler { get; } = x => Console.WriteLine(x.ToString().Replace("\n", "\n             "));
    public static void Write(LogEntry entry) => WriteHandler(entry);
    private static LogEntry WriteBack(LogEntry entry)
    {
      Write(entry);
      return entry;
    }

    public static LogEntry Write(Exception ex) => WriteBack(new LogEntry(Clock.Time, EnumerableEx.Single<Object>(ex)));
    public static LogEntry Write(string message, IEnumerable<object> data) => WriteBack(new LogEntry(Clock.Time, data.Concat(new Message(message)).ToList()));
    public static LogEntry Write(IEnumerable<object> data) => WriteBack(new LogEntry(Clock.Time, data));
    public static LogEntry Write(string message, params object[] data) => WriteBack(new LogEntry(Clock.Time, data.Concat(new Message(message)).ToList()));
    public static LogEntry StartOperation(string operationName, params object[] data) => new LogTemplate().StartOperation(operationName, data);
    public static LogEntry Catch(string message, Action action, params object[] data) => new LogTemplate().Catch(message, action, data);
    public static LogEntry Capture(string message, Action action, params object[] data) => new LogTemplate().Catch(message, action, data);
  }
}
