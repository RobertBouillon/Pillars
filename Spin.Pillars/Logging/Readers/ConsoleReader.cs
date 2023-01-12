using System;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.Logging.Readers;

public class ConsoleReader
{
  public bool ShowTime { get; set; }
  public int TimeWidth { get; set; } = 8;
  public string TimeFormat { get; set; }

  public bool ShowScope { get; set; } = true;
  public int ScopeWidth { get; set; } = 12;


  private Dictionary<string, MessageTemplate> _templates = new Dictionary<string, MessageTemplate>();

  public void Read(LogEntry entry)
  {
    if(ShowTime)
      Console.Write(SetWidth(entry.Time.ToString(TimeFormat), TimeWidth));

    if(ShowScope)
    {
      if (entry.HasScope)
        Console.Write(SetWidth(entry.Scope.ToString('\\'), ScopeWidth));
      else
        Console.Write(new string(' ', ScopeWidth));
    }

    if (entry.HasMessage)
    {
      var message = entry.Message.Text;
      if (!_templates.TryGetValue(message, out var template))
        _templates.Add(message, template = new MessageTemplate(entry.Message.Text));

      template.Compose(Console.Out, entry.Data.Take(template.Arguments));
    }

    Console.WriteLine();
  }

  private string SetWidth(string value, int width) =>
    value.Length > width ? value.Substring(0, width) :
    value.Length < width ? value.PadRight(width) :
    value;
}
