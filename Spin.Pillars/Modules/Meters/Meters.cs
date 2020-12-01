using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;
using Spin.Pillars.Logging;
using Spin.Pillars.Logging.Meters.Aggregate;
using Spin.Pillars.Logging.Meters.Simple;
using Spin.Pillars.Meters.Flow;

namespace Spin.Pillars.Meters
{
  public class Meters : CollectionBase<Meter>
  {
    #region Fields
    private readonly IModule _component;
    #endregion
    #region Properties
    public IModule Component => _component;
    #endregion

    #region Constructors
    public Meters(IModule component)
    {
      #region Validation
      if (component == null)
        throw new ArgumentNullException("component");
      #endregion
      _component = component;
    }
    #endregion

    #region Indexers
    public IMeter this[string name] => this.FirstOrDefault(x => x.Name == name);
    #endregion


    #region Methods
    public ISimpleMeter<T> Add<T>(string name, ISimpleMeter<T> parent)
    {
      lock (this)
      {
        ISimpleMeter<T> ret = FindCounter<T>(name);
        if (ret != null)
          return ret;

        switch (typeof(T).Name)
        {
          case "Int32":
            ret = (ISimpleMeter<T>)new ParentedIntMeter(name, (ISimpleMeter<int>)parent);
            break;
          case "Int64":
            ret = (ISimpleMeter<T>)new ParentedLongMeter(name, (ISimpleMeter<long>)parent);
            break;
          default:
            throw new Exception("Only counters of Int32 and Int64 are supported"); //Only two types supported by Interlocked.
        }

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }

    public IFlow<T> Add<T>(string name, IMeter<T> source, TimeSpan span, TimeSpan precision)
    {
      lock (this)
      {
        IFlow<T> ret = FindFlow<T>(name);
        if (ret != null)
          return ret;

        switch (typeof(T).Name)
        {
          case "UInt32":
            ret = (IFlow<T>)new UIntFlow(name, source as IMeter<uint>, span, precision, _component.Runtime.Heartbeat);
            break;
          case "Int32":
            ret = (IFlow<T>)new IntFlow(name, source as IMeter<int>, span, precision, _component.Runtime.Heartbeat);
            break;
          case "Int64":
            ret = (IFlow<T>)new LongFlow(name, source as IMeter<long>, span, precision, _component.Runtime.Heartbeat);
            break;
          default:
            throw new Exception("Only counters of Int32 and Int64 are supported"); //Only two types supported by Interlocked.
        }

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }

    public ISimpleMeter<T> Add<T>(string name)
    {
      lock (this)
      {
        ISimpleMeter<T> ret = FindCounter<T>(name);
        if (ret != null)
          return ret;

        switch (typeof(T).Name)
        {
          case "Int32":
            ret = (ISimpleMeter<T>)new IntMeter(name);
            break;
          case "Int64":
            ret = (ISimpleMeter<T>)new LongMeter(name);
            break;
          default:
            throw new Exception("Only counters of Int32 and Int64 are supported"); //Only two types supported by Interlocked.
        }

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }

    private ISimpleMeter<T> FindCounter<T>(string name)
    {
      ISimpleMeter<T> ret = null;
      IMeter metric = this[name];
      if (metric != null)
      {
        ret = metric as ISimpleMeter<T>;
        if (ret == null)
          throw new Exception(String.Format("A metric named '{0}' already exists, but it is of type '{1}', which is not a Counter.", name, metric.GetType()));
        //return ret;
      }
      return ret;
    }

    public IAggregate<TValue, TAggregate> Add<TValue, TAggregate>(string name)
    {
      lock (this)
      {
        var ret = FindAggregate<TValue, TAggregate>(name);
        if (ret != null)
          return ret;

        if (typeof(TValue) == typeof(Int32))
        {
          if (typeof(TAggregate) == typeof(Int64))
            ret = (IAggregate<TValue, TAggregate>)new IntLongMeter(name);
        }
        if (typeof(TValue) == typeof(Int64))
        {
          if (typeof(TAggregate) == typeof(Int64))
            ret = (IAggregate<TValue, TAggregate>)new IntLongMeter(name);
        }
        else if (typeof(TValue) == typeof(Single))
        {
          if (typeof(TAggregate) == typeof(Double))
            ret = (IAggregate<TValue, TAggregate>)new FloatDoubleMeter(name);
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new FloatDecimalMeter(name);
        }
        else if (typeof(TValue) == typeof(Double))
        {
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new DoubleDecimalMeter(name);
          if (typeof(TAggregate) == typeof(Double))
            ret = (IAggregate<TValue, TAggregate>)new DoubleDoubleMeter(name);
        }

        if (ret == null)
          throw new Exception(String.Format("Aggregate of type {0}/{1} not supported.", typeof(TValue).Name, typeof(TAggregate).Name));

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }

    public IAggregate<TValue, TAggregate> AddAggregate<TValue, TAggregate>(string name, TimeSpan span)
    {
      return AddAggregate<TValue, TAggregate>(name, span, TimeSpan.FromSeconds(span.TotalSeconds / 5));
    }

    public IAggregate<TValue, TAggregate> AddAggregate<TValue, TAggregate>(string name, TimeSpan span, TimeSpan resolution)
    {
      IClock clock = _component.Runtime.Clock;

      lock (this)
      {
        var ret = FindAggregate<TValue, TAggregate>(name);
        if (ret != null)
          return ret;

        if (typeof(TValue) == typeof(Int32))
        {
          if (typeof(TAggregate) == typeof(Int64))
            ret = (IAggregate<TValue, TAggregate>)new SlidingIntLongMeter(name, clock, span, resolution);
        }
        else if (typeof(TValue) == typeof(Single))
        {
          if (typeof(TAggregate) == typeof(Double))
            ret = (IAggregate<TValue, TAggregate>)new SlidingFloatDoubleMeter(name, clock, span, resolution);
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new SlidingFloatDecimalMeter(name, clock, span, resolution);
        }
        else if (typeof(TValue) == typeof(Double))
        {
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new SlidingDoubleDecimalMeter(name, clock, span, resolution);
        }

        if (ret == null)
          throw new Exception(String.Format("Aggregate of type {0}/{1} not supported.", typeof(TValue).Name, typeof(TAggregate).Name));

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }


    public IAggregate<TValue, TAggregate> Add<TValue, TAggregate>(string name, IAggregate<TValue, TAggregate> parent)
    {
      lock (this)
      {
        var ret = FindAggregate<TValue, TAggregate>(name);
        if (ret != null)
          return ret;

        if (typeof(TValue) == typeof(Int32))
        {
          if (typeof(TAggregate) == typeof(Int64))
            ret = (IAggregate<TValue, TAggregate>)new IntLongParentedMeter(name, (IAggregate<int, long>)parent);
        }
        else if (typeof(TValue) == typeof(Single))
        {
          if (typeof(TAggregate) == typeof(Double))
            ret = (IAggregate<TValue, TAggregate>)new FloatDoubleParentedMeter(name, (IAggregate<Single, Double>)parent);
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new FloatDecimalParentedMeter(name, (IAggregate<Single, Decimal>)parent);
        }
        else if (typeof(TValue) == typeof(Double))
        {
          if (typeof(TAggregate) == typeof(Decimal))
            ret = (IAggregate<TValue, TAggregate>)new DoubleDecimalParentedMeter(name, (IAggregate<Double, Decimal>)parent);
        }

        if (ret == null)
          throw new Exception(String.Format("Aggregate of type {0}/{1} not supported.", typeof(TValue).Name, typeof(TAggregate).Name));

        Add(ret);
        ret.Enabled = true;
        return ret;
      }
    }

    private IAggregate<TValue, TAggregate> FindAggregate<TValue, TAggregate>(string name)
    {
      IMeter metric = this[name];
      IAggregate<TValue, TAggregate> ret = null;
      if (metric != null)
      {
        ret = metric as IAggregate<TValue, TAggregate>;
        if (ret == null)
          throw new Exception(String.Format("A metric named '{0}' already exists, but it is of type '{1}', which is not an aggregate.", name, metric.GetType()));
        //return ret;
      }
      return ret;
    }

    private IFlow<T> FindFlow<T>(string name)
    {
      IMeter metric = this[name];
      IFlow<T> ret = null;
      if (metric != null)
      {
        ret = metric as IFlow<T>;
        if (ret == null)
          throw new Exception(String.Format("A metric named '{0}' already exists, but it is of type '{1}', which is not a flow.", name, metric.GetType()));
      }
      return ret;
    }

    public ITimeMeter AddTime(string name) 
    {
      var ret = new TimeMeter(name);
      Add(ret);
      return ret;
    }
    #endregion
  }
}
