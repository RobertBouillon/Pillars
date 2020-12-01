using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Aggregate
{
  public abstract class SimpleTracker<T>
  {
    protected SimpleTracker(string name)
    {
      Name = name;
    }

    public string Name { get; set; }

    public abstract void Track(T value);
    public abstract void Reset();
  }
}
