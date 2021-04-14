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
    public static Action<LogEntry> WriteHandler { get; } = x => Console.WriteLine(x);
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
    public static LogEntry Transition(string state, string value) => Write(EnumerableEx.Single<Object>(new State(state, value)));
    public static LogEntry Start(string state, params object[] data) => Write(data.Concat(new State(state, true)));
    public static LogEntry Finish(string state, params object[] data) => Write(data.Concat(new State(state, false)));
  }
}
