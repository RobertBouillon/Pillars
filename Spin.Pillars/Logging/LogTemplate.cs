using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging
{
  public class LogTemplate
  {
    public static Regex _tagParser = new Regex(@"\{([^\}]+)\}");
    public static Tag CreateTypeTag(Type type) => new Tag("Type", type.FullName);
    public List<Object> Data { get; set; }

    public IEnumerable<Tag> Tags => Data.OfType<Tag>();
    public IEnumerable<Label> Labels => Data.OfType<Label>();
    public Message Message => Data.OfType<Message>().SingleOrDefault();

    public LogTemplate(Type type, params object[] data) : this(data) => Data = Data.Concat(CreateTypeTag(type)).ToList();
    public LogTemplate(params object[] data) => Data = new List<object>();

    public LogEntry Write(Exception ex) => Log.Write(Data.Concat(ex));
    public LogEntry Write(string message, IEnumerable<object> data) => Log.Write(Data.Concat(data).Concat(new Message(message)));
    public LogEntry Write(IEnumerable<object> data) => Log.Write(Data.Concat(data));
    public LogEntry Write(string message, params object[] data) => Log.Write(Data.Concat(data).Concat(new Message(message)));
    public LogEntry Write(params object[] data) => Log.Write(Data.Concat(data));
    public LogEntry StartOperation(string operationName, params object[] data) => Write(data.Append(new Tag("Operation", operationName)).Append(new Tag("Status", Log.StartingOperationStatus)));
    public virtual LogEntry Error(params object[] data) => Write(data.Append(Log.ErrorData));

    public LogEntry Catch(string message, Action action, params object[] data)
    {
      var op = StartOperation(message, data);
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

    public LogEntry Capture(string message, Action action, params object[] data)
    {
      var op = StartOperation(message, data);
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

    //public LogEntry Finish(string state, params object[] data) => Log.Write(Data.Concat(data).Concat(new State(state, false)));

    public override string ToString()
    {
      var message = Message.Text;
      var builder = new StringBuilder();
      bool hasMessage = false;

      if (message is null && Tags.Any(x => x.Text == "Operation"))
        message = Tags.First(x => x.Text == "Operation").Value.ToString();

      var embeddedTags = new List<String>();
      if (message is not null)
      {
        embeddedTags.AddRange(ParseEmbeddedTags(message));
        var tags = Tags.ToDictionary(x => x.Text, x => x.Value.ToString());
        builder.Append(message);
        foreach (var tag in embeddedTags)
          if (tags.TryGetValue(tag, out var value))
            builder.Replace($"{{{tag}}}", value);
        hasMessage = true;
      }

      var data = Data.Except(EnumerableEx.Single((object)Message)).Except(Tags.Where(x => x.Text == "Type" || x.Text == "Operation").Cast<object>()).ToList();
      data.Remove(x => x is Tag tag && embeddedTags.Contains(tag.Text));

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

    private IEnumerable<string> ParseEmbeddedTags(string message) => _tagParser.Matches(message.Remove("{{").Remove("}}")).Select(x => x.Groups[1].Value);
  }
}
