using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.Logging;

public class LogScope
{
  public LogScope Parent { get; set; }
  public string Name { get; }
  public List<object> Data { get; set; }
  public DateTime Created { get; set; }
  public DateTime LastUpdate { get; set; }
  public TimeSpan Elapsed => Log.Clock.Time - LastUpdate;
  public Path Path => new Path(this.Traverse(x => x.Parent).Reverse().Skip(1).Select(x => x.Name));

  internal LogScope() => LastUpdate = Created = Log.Clock.Time;
  internal LogScope(string name) : this() => Name = name;
  internal LogScope(string name, IEnumerable<object> data) : this(name) => Data = data.ToList();
  internal LogScope(string name, IEnumerable<object> data, LogScope parent) : this(name) => Parent = parent;

  public LogScope AddScope(string name, params object[] data) => new LogScope(name, data, this);

  public LogScope Start(string name, params object[] data)
  {
    var log = new LogScope(name) { Parent = this };
    var datalist = data.Concat(new Scope(Path));

    if (!data.OfType<Tag>().Any(x => x.Text == "Status"))
      datalist = datalist.Concat(new Tag("Status", "Started"));

    log.Write(datalist);

    return log;
  }

  public LogEntry Catch(string scope, Action action, params object[] data)
  {
    var op = Start(scope, data);
    try
    {
      action();
      return op.Finish();
    }
    catch (Exception ex)
    {
      return op.Failed(data.Append(ex).Append(Log.ErrorData));
    }
  }

  public LogEntry Capture(string scope, Action action, params object[] data)
  {
    var op = Start(scope, data);
    try
    {
      action();
      return op.Finish();
    }
    catch (Exception ex)
    {
      op.Failed(data.Append(ex).Append(Log.ErrorData));
      throw;
    }
  }

  public LogEntry Error(params object[] data) => Write(data.Append(Log.ErrorData));

  public LogEntry Write(Exception ex) => Log.Write(new Scope(Path), ex);
  public LogEntry Write(string message, IEnumerable<object> data) => Log.Write(Path, MakeTags(message, data).Cast<Object>().Append(new Message(message)));
  public LogEntry Write(IEnumerable<object> data) => Log.Write(Path, data);
  public LogEntry Write(string message, params object[] data) => Log.Write(Path, MakeTags(message, data).Cast<Object>().Append(new Message(message)));
  public LogEntry Write(params object[] data) => Log.Write(Path, data);

  public LogEntry Update(string status, params object[] data) => Update(status, (IEnumerable<object>)data);
  public LogEntry Update(string status, IEnumerable<object> data)
  {
    var ret = Log.Write(Path, data
      .Append(new Tag("Status", status))
      .Append(new Tag("Elapsed", Elapsed)));
    LastUpdate = Log.Clock.Time;
    return ret;
  }

  public LogEntry Start(params object[] data) => Update("Started", data);
  public LogEntry Failed(params object[] data) => Update("Failed", data.Append(Log.ErrorData));
  public LogEntry Finish(params object[] data) => Update(Log.FinishedOperationStatus, data);
  public IEnumerable<Tag> MakeTags(string message, IEnumerable<object> data)
  {
    int index = 0;
    bool open = false;
    int start = 0;
    var e = message.GetEnumerator();
    var datas = data.ToQueue();
    while (e.MoveNext())
    {
      if (open && e.Current == '}')
      {
        yield return new Tag(message.Substring(start, index - start), datas.Dequeue());
        open = false;
      }
      else if (!open && e.Current == '{')
      {
        start = index + 1;
        open = true;
      }
      index++;
    }
  }
}
