using System;

namespace Spin.Pillars.Logging.Writers
{
  public class ConsoleWriter : TextLogWriter
  {
    public ConsoleWriter() : base(Console.Out) =>
      Formatter = x => String.Format(@"{0:mm\:ss\:ff} {1,-20} {2}", x.EntryTime, x.Module.Name, x.ToString(), Delimiter);
  }
}
