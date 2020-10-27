using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Specialized
{
  public class CpuMeter : PerformanceCounterMeter
  {
    public CpuMeter(string name, string instanceName) : base(name, "Process", "% Processor Time", instanceName)
    {
    }

  }
}
