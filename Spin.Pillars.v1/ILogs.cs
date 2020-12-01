using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public interface ILogs : IList<ILog>
  {
    ILog this[string name] { get; }
  }
}
