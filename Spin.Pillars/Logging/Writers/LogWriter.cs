using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spin.Pillars.Logging.Writers
{
  public abstract class LogWriter
  {
    public virtual void Write(LogEntry entry) => Write(EnumerableEx.Single(entry));
    public abstract void Write(IEnumerable<LogEntry> entries);
  }
}
