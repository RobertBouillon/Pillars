using Spin.Pillars.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Modules.Logging.Writers
{
  public class FilteredWriter : LogWriter
  {
    public Func<LogEntry, bool> Filter { get; set; }
    public LogWriter Writer { get; set; }

    public FilteredWriter(LogWriter writer, Func<LogEntry, bool> filter)
    {
      #region Validation
      if (writer is null)
        throw new ArgumentNullException(nameof(writer));
      if (filter is null)
        throw new ArgumentNullException(nameof(filter));
      #endregion
      Writer = writer;
      Filter = filter;
    }

    public override void Write(IEnumerable<LogEntry> entries) => Writer.Write(entries.Where(Filter));
  }
}
