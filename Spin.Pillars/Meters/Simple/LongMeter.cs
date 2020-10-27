using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Modules.Logging.Meters.Simple
{
  public class LongMeter : SimpleMeter<long>
  {
    #region Fields
    private bool _enabled;
    private long _value;
    #endregion

    #region Constructors
    public LongMeter(string name) : base(name)
    {

    }
    #endregion
    public override long Increment()
    {
      return Interlocked.Increment(ref _value);
    }

    public override long Decrement()
    {
      return Interlocked.Decrement(ref _value);
    }

    public override long Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public override void Reset()
    {
      _value = 0;
    }

    public override long Increment(long value)
    {
      return _enabled ? Interlocked.Add(ref _value, value) : 0;
    }

    public override long Decrement(long value)
    {
      return _enabled ? Interlocked.Add(ref _value, -value) : 0;
    }

    public override bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }
  }
}
