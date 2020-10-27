using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface ILogReader : IEnumerable<ILogEntry>
  {
    IEnumerable<ILogEntry> Read(int max);
  }
}
