using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spin.Pillars.Logging;

public class LogEntry
{
  public Path Scope => HasScope ?
    (Path)Data.OfType<Tag>().First(x => x.Text == "Scope").Value :
    null;

  public DateTime Time { get; set; }
  public TimeSpan Elapsed => Log.Clock.Time - Time;

  public static Regex _tagParser = new Regex(@"\{([^\}]+)\}");
  public static Tag CreateTypeTag(Type type) => new Tag("Type", type.FullName);
  public List<Object> Data { get; set; }

  public IEnumerable<Tag> Tags => Data.OfType<Tag>();
  public IEnumerable<Label> Labels => Data.OfType<Label>();
  public Message Message => Data.OfType<Message>().SingleOrDefault();
  public bool HasMessage => Data.OfType<Message>().Any();
  public bool HasScope => Data.OfType<Tag>().Any(x => x.Text == "Scope");

  public LogEntry(DateTime time, Path scope, IEnumerable<object> data) => (Time, Data) = (time, data.Concat(new Tag("Scope", scope)).ToList());
  //public LogEntry Error(params object[] data) => throw new NotImplementedException();
  //public LogEntry Close(params object[] data) => throw new NotImplementedException();

  //public override string ToString() => Time.ToString(@"hh\:mm\:ss\.fff") + ' ' + base.ToString();

  public bool HasTag(string label) => Data.OfType<Tag>().Any(x => x.Text == label);
  public T GetTag<T>(string label) => (T)this[label];
  public Attempt<T> TryGetTag<T>(string label)
  {
    foreach (var datum in Data.OfType<Tag>())
      if (datum.Text == label)
        return (T)datum.Value;

    return new("Tag not found");
  }

  public object this[string label] => Data.OfType<Tag>().First(x => x.Text == label).Value;
}
