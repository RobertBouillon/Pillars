using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;
using System.Threading;

namespace Spin.Pillars.Logging.Meters.Simple
{
  public class ParentedLongMeter: SimpleMeter<long>
  {

    #region Fields
    private long _value;
    private ISimpleMeter<long> _parent;
    private bool _enabled;
    #endregion

    #region Properties
    public override bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }
    #endregion

    #region Constructors
    public ParentedLongMeter(string name, ISimpleMeter<long> parent)
      : base(name)
    {
      #region Validation
      if (parent == null)
        throw new ArgumentNullException("parent");
      #endregion
      _parent = parent;
    }
    #endregion

    #region Methods
    public override long Increment()
    {
      if (!_enabled)
        return _value;
      _parent.Increment();
      return Interlocked.Increment(ref _value);
    }

    public override long Decrement()
    {
      if (!_enabled)
        return _value;

      _parent.Decrement();
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
      if (!_enabled)
        return _value;

      _parent.Increment(value);
      return Interlocked.Add(ref _value, value);
    }

    public override long Decrement(long value)
    {
      if (!_enabled)
        return _value;
      _parent.Decrement(value);
      return Interlocked.Add(ref _value, -value);
    }
    #endregion
  }
}
