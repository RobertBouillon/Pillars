using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Readers;

public class TypedLogEntry
{
  public DateTime Time { get; }
  public IEnumerable<Object> Data { get; }

  public TypedLogEntry(LogEntry entry)
  {
    Time = entry.Time;
    Data = entry.Data;
  }
}
