using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Specialized
{
  public class ClrMemoryUsageMeter : PerformanceCounterMeter
  {
    public ClrMemoryUsageMeter(string name, string instanceName) 
      :base(name,".NET CLR Memory","# Bytes in all Heaps", instanceName)
    {

    }
  }
}
