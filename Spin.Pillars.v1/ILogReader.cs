using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public interface ILogReader : IEnumerable<ILogEntry>
  {
    IEnumerable<ILogEntry> Read(int max);
  }
}
