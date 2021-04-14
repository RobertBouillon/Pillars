using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Readers
{
  public class ConsoleReader
  {
    public void Read(LogEntry entry) => Console.WriteLine(entry);
  }
}
