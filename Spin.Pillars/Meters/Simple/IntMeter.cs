using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Modules.Logging.Meters.Simple
{
  public class IntMeter : SimpleMeter<int>
  {
    private int _value;
    private bool _enabled;

    public override bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }

    public IntMeter(string name) : base(name)
    {

    }

    public override int Increment()
    {
      return Interlocked.Increment(ref _value);
    }

    public override int Decrement()
    {
      return Interlocked.Decrement(ref _value);
    }

    public override void Reset()
    {
      _value = 0;
    }

    public override int Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public override int Increment(int value)
    {
      return _enabled ? Interlocked.Add(ref _value, value) : 0;
    }

    public override int Decrement(int value)
    {
      return _enabled ? Interlocked.Add(ref _value, -value) : 0;
    }
  }
}
