using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Logging
{
  public class LogEntry : LogTemplate
  {
    public DateTime Time { get; set; }
    public TimeSpan Elapsed => Log.Clock.Time - Time;

    public LogEntry(DateTime time, IEnumerable<object> data) => (Time, Data) = (time, data.ToList());
    //public LogEntry Error(params object[] data) => throw new NotImplementedException();
    //public LogEntry Close(params object[] data) => throw new NotImplementedException();

    public LogEntry Finish(params object[] data) => Update("Finished", data);
    public LogEntry Update(string status, params object[] data) =>
      Log.Write(Data
        .Except(x => x is Tag tag && (tag.Text == "Status" || tag.Text == "Elapsed"))
        .Append(new Tag("Status", status))
        .Append(new Tag("Elapsed", Log.Clock.Time - Time))
        .Concat(data));

    public LogEntry Failed(params object[] data) => Update("Failed", data.Append(Log.ErrorData));

    public override string ToString() => Time.ToString(@"hh\:mm\:ss\.fff") + ' ' + base.ToString();
  }
}
