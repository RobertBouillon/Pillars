using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging
{
  public abstract class LogWriter
  {
    public abstract void Write(LogEntry entry);
  }
}
