using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Specialized
{
  public class PerformanceCounterMeter : IMeter
  {
    private PerformanceCounter _counter;
    private string _name;
    private bool _enabled;

    public bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }

    public PerformanceCounterMeter(string name, string categoryName, string counterName, string instanceName)
    {
      _name = name;

      _counter = new PerformanceCounter();
      _counter.CategoryName = categoryName;
      _counter.CounterName = counterName;
      _counter.InstanceName = instanceName;
    }

    public string Name
    {
      get { return _name; }
    }

    public object Value
    {
      get { return _enabled ? _counter.NextValue() : 0; }
      set { throw new NotSupportedException(); }
    }

    public void Reset()
    {
    }
  }
}
