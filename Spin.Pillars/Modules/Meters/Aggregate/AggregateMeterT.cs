using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public abstract class AggregateMeter<TValue, TAggregate> : IAggregate<TValue, TAggregate>
  {
    #region Fields
    private string _name;
    private TValue _last;
    private TAggregate _sum;
    private long _count;
    private TValue _max;
    private TValue _min;
    private bool _enabled;
    #endregion

    #region Properties

    public bool Enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public TValue Last => _last;
    public TAggregate Average => Divide(_sum, _count);
    public long Count => _count;
    public TValue Max => _max;
    public TValue Min => _min;

    #endregion

    #region Constructors
    protected AggregateMeter(string name)
    {
      #region Validation
      if (name == null)
        throw new ArgumentNullException("name");
      #endregion
      _name = name;
    }
    #endregion
    #region Methods
    public virtual void Record(TValue value)
    {
      if (!_enabled)
        return;

      ++_count;
      _last = value;
      _sum = Add(_sum, value);
      _max = CalculateMax(_max, value);
      _min = CalculateMin(_min, value);
    }

    public void Reset()
    {
      _count = 0;
      _last = default(TValue);
      _sum = default(TAggregate);
      _max = default(TValue);
      _min = default(TValue);
    }

    protected abstract TAggregate Add(TAggregate a, TValue b);
    protected abstract TAggregate Divide(TAggregate a, long b);
    protected abstract TValue CalculateMin(TValue a, TValue b);
    protected abstract TValue CalculateMax(TValue a, TValue b);
    #endregion

    #region Explicit Implementations
    TValue IAggregate<TValue, TAggregate>.LastValue => _last;
    TValue IAggregate<TValue, TAggregate>.Max => _max;
    TValue IAggregate<TValue, TAggregate>.Min => _min;
    TAggregate IAggregate<TValue, TAggregate>.Average => Divide(_sum, _count);
    TAggregate IAggregate<TValue, TAggregate>.Sum => _sum;
    object IAggregate.LastValue => _last;
    object IAggregate.Max => _max;
    object IAggregate.Min => _min;
    object IAggregate.Average => Divide(_sum, _count);
    object IAggregate.Sum => _sum;
    long IAggregate.Count => _count;
    void IAggregate.Record(object value) => Record((TValue)value);
    string IMeter.Name => _name;

    object IMeter.Value
    {
      get { return Divide(_sum, _count); }
      set { throw new NotSupportedException(); }
    }
    #endregion
  }
}
