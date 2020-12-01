using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Spin.Pillars.Logging
{
  public class LogEntry
  {
    public Path Category { get; set; }
    public object Data { get; set; }
    public DateTime EntryTime { get; set; }
    public string FormatText { get; set; }
    public object[] Arguments { get; set; }
    public ExceptionInfo Error { get; set; }
    public LogSeverity Severity { get; set; }

    public String FormattedString => String.Format(FormatText, Arguments);

    public LogEntry() { }

    public override string ToString() => FormattedString;
  }
}
