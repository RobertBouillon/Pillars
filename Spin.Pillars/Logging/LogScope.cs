using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging
{
  public class LogScope
  {
    public LogScope Parent { get; set; }
    public string Name { get; }
    //public List<object> Data { get; set; }
    public DateTime Created { get; set; }
    public TimeSpan Elapsed => Log.Clock.Time - Created;
    public Path Path => new Path(this.Traverse(x => x.Parent).Reverse().Skip(1).Select(x => x.Name));

    //public IEnumerable<Tag> Tags => Data.OfType<Tag>();
    //public IEnumerable<Label> Labels => Data.OfType<Label>();
    //public Message Message => Data.OfType<Message>().SingleOrDefault();

    internal LogScope() => Created = Log.Clock.Time;
    internal LogScope(string name) : this() => Name = name;
    internal LogScope(string name, LogScope parent) : this(name) => Parent = parent;

    public LogScope AddScope(string name, params object[] data) => new LogScope(LogEntry.Interpolate(name, data.OfType<Tag>()), this);

    public LogScope Start(string name, params object[] data)
    {
      var log = new LogScope(LogEntry.Interpolate(name, data.OfType<Tag>()), this);
      var datalist = data.ToList();

      if (!data.OfType<Tag>().Any(x => x.Text == "Status"))
        datalist.Add(new Tag("Status", "Started"));

      //if (!data.OfType<Message>().Any())
      //  datalist.Add(new Message("{Scope:leaf} - {Status}"));

      log.Write(datalist);

      return log;
    }

    public LogEntry Start(params object[] data) => Log.Write(data.Append(new Tag("Status", "Started")));

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

    public LogEntry Write(Exception ex) => Log.Write(Path, ex);
    public LogEntry Write(string message, IEnumerable<object> data) => Log.Write(Path, data.Append(new Message(message)));
    public LogEntry Write(IEnumerable<object> data) => Log.Write(Path, data);
    public LogEntry Write(string message, params object[] data) => Log.Write(Path, data.Append(new Message(message)));
    public LogEntry Write(params object[] data) => Log.Write(Path, data);

    public LogEntry Update(string status, params object[] data) => Update(status, (IEnumerable<object>)data);
    public LogEntry Update(string status, IEnumerable<object> data) =>
      Log.Write(Path, data
        .Append(new Tag("Status", status))
        .Append(new Tag("Elapsed", Elapsed)));

    public LogEntry Failed(params object[] data) => Update("Failed", data.Append(Log.ErrorData));
    public LogEntry Finish(params object[] data) => Update(Log.FinishedOperationStatus, data);
  }
}
