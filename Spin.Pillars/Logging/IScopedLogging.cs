using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging
{
  public interface IScopedLogging
  {
    LogScope Log { get; }
  }
}
