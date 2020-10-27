using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;

namespace System.Modules.Logging.Watchers
{
  public class ConsoleLogWatcher : TextWriterLogWatcher
  {
    public ConsoleLogWatcher() : base(Console.Out) =>
      Formatter = x => String.Format(@"{0:mm\:ss\:ff} {1,-20} {2}", x.EntryTime, x.Module.Name, x.ToString(), Delimiter);
  }
}
