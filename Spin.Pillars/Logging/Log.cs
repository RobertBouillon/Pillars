using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Data;
using Spin.Pillars.Logging.Readers;
using Spin.Pillars.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.Logging;

public static class Log
{
  public static IClock Clock { get; } = new Clock();
  public static LogScope DefaultScope { get; } = new LogScope();
  public static LogScope Start(string name, params object[] data) => DefaultScope.Start(name, data);
  //public static void Capture(string name, Action<LogScope> action);

  public static object ErrorData => new Label("Error");
  public static string StartingOperationStatus => "Started";
  public static string FinishedOperationStatus => "Finished";

  public static Action<LogEntry> Writer { get; set; }
  public static void Write(LogEntry entry) => Writer(entry);
  private static LogEntry WriteBack(LogEntry entry)
  {
    Write(entry);
    return entry;
  }

  static Log()
  {
    var reader = new ConsoleReader();
    Writer = x => reader.Read(x);
  }

  public static LogEntry Write(params object[] data) => WriteBack(new LogEntry(Clock.Time, Path.Empty, data));
  public static LogEntry Write(string message, params object[] data) => WriteBack(new LogEntry(Clock.Time, Path.Empty, data.Concat(new Message(message)).ToList()));

  public static LogEntry Write(Path scope, Exception ex) => WriteBack(new LogEntry(Clock.Time, scope, EnumerableEx.Single<Object>(ex)));
  public static LogEntry Write(Path scope, string message, IEnumerable<object> data) => WriteBack(new LogEntry(Clock.Time, scope, data.Concat(new Message(message)).ToList()));
  public static LogEntry Write(Path scope, IEnumerable<object> data) => WriteBack(new LogEntry(Clock.Time, scope, data));
  public static LogEntry Write(Path scope, string message, params object[] data) => WriteBack(new LogEntry(Clock.Time, scope, data.Concat(new Message(message)).ToList()));

  //public static LogEntry StartOperation(string operationName, params object[] data) => new LogTemplate().StartOperation(operationName, data);
  //public static LogEntry Catch(string message, Action action, params object[] data) => new LogTemplate().Catch(message, action, data);
  //public static LogEntry Capture(string message, Action action, params object[] data) => new LogTemplate().Catch(message, action, data);
}
