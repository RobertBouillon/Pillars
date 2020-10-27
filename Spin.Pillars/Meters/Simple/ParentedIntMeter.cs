using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;
using System.Threading;

namespace System.Modules.Logging.Meters.Simple
{
  public class ParentedIntMeter : SimpleMeter<int>
  {
    #region Fields
    private int _value;
    private ISimpleMeter _parent;
    private bool _enabled;
    #endregion

    #region Properties
    public override bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }
    #endregion

    public ParentedIntMeter(string name, ISimpleMeter<int> parent)
      : base(name)
    {
      #region Validation
      if (parent == null)
        throw new ArgumentNullException("parent");
      #endregion
      _parent = parent;
    }

    public override int Increment()
    {
      if (!_enabled)
        return _value;
      _parent.Increment();
      return Interlocked.Increment(ref _value);
    }

    public override int Decrement()
    {
      if (!_enabled)
        return _value;
      _parent.Decrement();
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
      if (!_enabled)
        return _value;
      _parent.Increment(value);
      return Interlocked.Add(ref _value, value);
    }

    public override int Decrement(int value)
    {
      if (!_enabled)
        return _value;
      _parent.Increment(value);
      return Interlocked.Add(ref _value, -value);
    }
  }
}
