using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Modules.Logging.Writers
{
  public class ConsoleWriter : TextLogWriter
  {
    public ConsoleWriter() : base(Console.Out) =>
      Formatter = x => String.Format(@"{0:mm\:ss\:ff} {1,-20} {2}", x.EntryTime, x.Module.Name, x.ToString(), Delimiter);
  }
}
