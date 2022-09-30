using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Spin.Pillars.Logging
{
  public class LogEntry
  {
    public Path Scope { get; set; }
    public DateTime Time { get; set; }
    public TimeSpan Elapsed => Log.Clock.Time - Time;

    public static Regex _tagParser = new Regex(@"\{([^\}]+)\}");
    public static Tag CreateTypeTag(Type type) => new Tag("Type", type.FullName);
    public List<Object> Data { get; set; }

    public IEnumerable<Tag> Tags => Data.OfType<Tag>();
    public IEnumerable<Label> Labels => Data.OfType<Label>();
    public Message Message => Data.OfType<Message>().SingleOrDefault();
    public bool HasMessage => Data.OfType<Message>().Any();
    public bool HasScope => Data.OfType<Tag>().Any(x=>x.Text == "Scope");

    public LogEntry(DateTime time, Path scope, IEnumerable<object> data) => (Time, Scope, Data) = (time, scope, data.ToList());
    //public LogEntry Error(params object[] data) => throw new NotImplementedException();
    //public LogEntry Close(params object[] data) => throw new NotImplementedException();

    //public override string ToString() => Time.ToString(@"hh\:mm\:ss\.fff") + ' ' + base.ToString();
  }
}
