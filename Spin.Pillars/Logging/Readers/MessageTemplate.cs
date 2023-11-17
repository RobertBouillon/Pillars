using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Spin.Pillars.Logging.Data;

namespace Spin.Pillars.Logging.Readers;

public partial class MessageTemplate
{
  struct Part
  {
    public bool IsLiteral;
    public int Length;

    public Part(bool isLiteral, int length) => (IsLiteral, Length) = (isLiteral, length);

    public bool HasFormat(string text, int index) => text.AsSpan(index, Length).IndexOf(':') >= 0;
    public string Label(string text, int index)
    {
      var span = text.AsSpan(index, Length);
      var colon = span.IndexOf(':');
      if (colon >= 0)
        return span.Slice(0, colon).ToString();
      else
        return span.ToString();
    }

    public string FormatString(string text, int index)
    {
      var span = text.AsSpan(index, Length);
      var colon = span.IndexOf(':');
      if (colon >= 0)
        return span.Slice(colon + 1, Length - colon - 2).ToString();
      else
        return span.ToString();
    }
  }

  private List<Part> _parts;
  private string _text;
  public int Arguments => _parts.Count(x => !x.IsLiteral);

  public MessageTemplate(string message)
  {
    List<Part> parts = new List<Part>();

    int index = 0;
    int size = message.Length;
    var text = message.AsSpan();
    _text = message;
    while (index < size)
    {
      Part part;
      if (text[index] == '{')
      {
        int len = message.AsSpan(index + 1).IndexOf('}') + 2;
        if (len == 0 || len == -1)
          throw new Exception($"Substitution label expected at position {index + 1}");
        part = new Part(false, len);
      }
      else
      {
        int len = message.AsSpan(index + 1).IndexOf('{');
        if (len == -1)
          len = size - index;
        else
          len += 1;

        part = new Part(true, len);
      }
      index += part.Length;
      parts.Add(part);
    }
    _parts = parts;
  }

  public string Compose(object[] args)
  {
    var builder = new StringBuilder();
    using (var writer = new StringWriter(builder))
    {
      Compose(writer, args);
      writer.Flush();
    }
    return builder.ToString();
  }

  public void Compose(TextWriter writer, IEnumerable<object> args)
  {
    int index = 0;
    var a = args.ToQueue();
    foreach (var part in _parts)
    {
      if (part.IsLiteral)
        writer.Write(_text.AsSpan(index, part.Length));
      else
      {
        if (part.HasFormat(_text, index))
          writer.Write(((IFormattable)a.Dequeue()).ToString(part.FormatString(_text, index), CultureInfo.CurrentCulture));
        else
        {
          var output = a.Dequeue();
          if(output is Tag tag)
            writer.Write(tag.Value);
          else
            writer.Write(output);
        }
      }

      index += part.Length;
    }
  }
}
