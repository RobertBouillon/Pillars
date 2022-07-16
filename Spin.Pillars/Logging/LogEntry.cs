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
<<<<<<< Updated upstream

    public override string ToString()
    {
      var message = Message.Text;
      var builder = new StringBuilder();
      bool hasMessage = false;

      builder.Append(Time.ToString(@"hh\:mm\:ss\.fff"));
      builder.Append(' ');

      if (message is null && Tags.Any(x => x.Text == "Status"))
        message = "{Scope:leaf} - {Status}";

      List<string> observedTags = new List<string>();
      if (hasMessage = message is not null)
        Interpolate(message, Tags.Append(new Tag("Scope", Scope)), builder, out observedTags);

      var data = Data.Except(EnumerableEx.Single((object)Message)).Except(Tags.Where(x => x.Text == "Type").Cast<object>()).ToList();
      data.Remove(x => x is Tag tag && observedTags.Contains(tag.Text));

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



    //private void Interpolate(string message, StringBuilder builder, out List<string> observedTags)
    internal static string Interpolate(string message, IEnumerable<Tag> tags)
    {
      var builder = new StringBuilder();
      Interpolate(message, tags, builder, out var _);
      return builder.ToString();
    }

    private static void Interpolate(string message, IEnumerable<Tag> tags1, StringBuilder builder, out List<string> observedTags)
    {
      observedTags = new List<String>();
      var tags = tags1.ToDictionary(x => x.Text, x => x.Value);

      var index = 0;
      for (int i = 0; i < message.Length; i++)
      {
        if (message[i] == '{' && message.Length > i)
        {
          if (message[i + 1] == '{')
            i++;
          else
          {
            builder.Append(message, index, i - index);

            for (int x = i; x < message.Length; x++)
            {
              if (message[x] == '}')
              {
                var tagStart = i + 1;
                var tagLen = x - i - 1;
                int formatStart = 0, formatLen = 0;

                for (int z = tagStart; z < tagStart + tagLen; z++)
                {
                  if (message[z] == ':')
                  {
                    formatLen = tagLen - z;
                    formatStart = z + 1;
                    tagLen = z - tagStart;
                    break;
                  }
                }

                var tag = message.Substring(tagStart, tagLen);
                observedTags.Add(tag);
                if (tags.TryGetValue(tag, out var value))
                {
                  if (formatLen > 0 && value is IFormattable formattable)
                    builder.Append(formattable.ToString(message.Substring(formatStart, formatLen), null));
                  else
                    builder.Append(value);
                }
                else
                  builder.Append(message, index, x - index + 1);

                index = x + 1;
                break;
              }
            }
          }
        }
      }

      if (index < message.Length)
        builder.Append(message, index, message.Length - index);
    }
=======
>>>>>>> Stashed changes
  }
}
