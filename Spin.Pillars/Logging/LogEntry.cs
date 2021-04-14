using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Logging
{
  public class LogEntry
  {
    public DateTime Time { get; set; }
    public List<Object> Data { get; set; }

    public IEnumerable<Tag> Tags => Data.OfType<Tag>();
    public IEnumerable<Label> Labels => Data.OfType<Label>();
    public IEnumerable<State> States => Data.OfType<State>();
    public Message Message => Data.OfType<Message>().SingleOrDefault();

    public LogEntry(DateTime time, IEnumerable<object> data) => (Time, Data) = (time, data.ToList());
    public override string ToString()
    {
      var message = Message.Text;
      var builder = new StringBuilder();
      bool hasMessage = false;

      builder.Append(Time.ToString(@"hh\:mm\:ss\.fff"));
      builder.Append(' ');

      if (message is not null)
      {
        hasMessage = true;
        var tags = Tags.ToDictionary(x => x.Text, x => x.Value);
        builder.Append(message);
        foreach (var tag in tags)
          builder.Replace($"{{{tag.Key}}}", tag.Value.ToString());
        hasMessage = true;
      }

      var data = Data.Except(EnumerableEx.Single((object)Message)).ToList();
      if (data.Any())
      {
        if (hasMessage)
          builder.Append(" (");

        foreach (var meta in data)
        {
          builder.Append(meta);
          builder.Append(", ");
        }

        if (hasMessage)
        {
          builder.Remove(builder.Length - 2, 2);
          builder.Append(')');
        }
      }

      return builder.ToString();
    }
  }
}
