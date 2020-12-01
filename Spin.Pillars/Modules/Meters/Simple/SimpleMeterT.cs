using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;

namespace Spin.Pillars.Logging.Meters.Simple
{
  public abstract class SimpleMeter<T> : ISimpleMeter<T>, IMeter
  {
    protected SimpleMeter(string name)
    {
      Name = name;
    }

    public string Name { get; set; }


    public abstract bool Enabled { get; set; }
    public abstract T Increment();
    public abstract T Decrement();
    public abstract T Increment(T value);
    public abstract T Decrement(T value);
    public abstract T Value { get; set; }
    public abstract void Reset();

    #region Expicit ISimpleCounter Implementation
    object ISimpleMeter.Increment()
    {
      return Increment();
    }

    object ISimpleMeter.Decrement()
    {
      return Decrement();
    }

    object ISimpleMeter.Value
    {
      get { return Value; }
    }
    #endregion

    #region Explicit IMetric Implementation
    string IMeter.Name
    {
      get { return Name; }
    }

    object IMeter.Value
    {
      get { return Value; }
      set { Value = (T)value; }
    }

    

    void IMeter.Reset()
    {
      Reset();
    }
    #endregion


    object ISimpleMeter.Increment(object value)
    {
      return Increment((T)value);
    }

    object ISimpleMeter.Decrement(object value)
    {
      return Decrement((T)value);
    }

  }
}
