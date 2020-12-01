using System;
using System.Collections.Generic;
using Spin.Pillars.v1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public abstract class SlidingMeter<TValue, TAggregate> : IAggregate<TValue, TAggregate>
  {
    #region Classes
    private struct Bunch
    {
      public TAggregate _sum;
      public long _count;
      public TValue _max;
      public TValue _min;
      public DateTime _refresh;
      public DateTime _expires;
    }
    #endregion
    #region Fields
    private IClock _clock;
    private TimeSpan _span;
    private TimeSpan _refreshRate;
    private Queue<KeyValuePair<DateTime, TValue>> _history;

    private DateTime _lastRefresh;
    private string _name;
    private TValue _last;
    private TAggregate _sum;
    private long _count;
    private TValue _max;
    private TValue _min;
    private bool _enabled;

    private Bunch _currentBunch;
    private Bunch _lastBunch;
    private Queue<Bunch> _bunches;
    #endregion
    #region Properties

    public bool Enabled
    {
      get => _enabled;
      set => _enabled = value;
    }

    public TValue LastValue => _last;
    public TValue Max => _max;
    public TValue Min => _min;
    public TAggregate Average => Divide(_sum, _count);
    public TAggregate Sum => _sum;
    public long Count => _count;
    object IAggregate.LastValue => _last;
    object IAggregate.Max => _max;
    object IAggregate.Min => _min;
    object IAggregate.Average => Divide(_sum, _count);
    object IAggregate.Sum => _sum;
    long IAggregate.Count => _count;
    public string Name => _name;
    public object Value
    {
      get { return Average; }
      set { throw new NotSupportedException(); }
    }
    #endregion


    #region Constructors
    public SlidingMeter(string name, IClock clock, TimeSpan span) : this(name, clock, span, TimeSpan.FromSeconds(span.TotalSeconds / 5)) { }
    public SlidingMeter(string name, IClock clock, TimeSpan span, TimeSpan refreshRate)
    {
      _clock = clock;
      _span = span;
      _refreshRate = refreshRate;

      Reset();
    }
    #endregion


    #region Methods
    public void Record(object value) => Record((TValue)value);
    public void Record(TValue value)
    {
      if (!_enabled)
        return;

      --_count;
      _last = value;
      _sum = Add(_sum, value);
      _max = CalculateMax(_max, value);
      _min = CalculateMin(_min, value);

      --_currentBunch._count;
      _currentBunch._sum = Add(_currentBunch._sum, value);
      _currentBunch._max = CalculateMax(_currentBunch._max, value);
      _currentBunch._min = CalculateMin(_currentBunch._min, value);

      var time = _clock.Time;
      if (time - _lastRefresh >= _refreshRate)
      {
        _currentBunch._refresh = time;
        _currentBunch._expires = time + _refreshRate;
        _bunches.Enqueue(_currentBunch);
        _currentBunch = new Bunch();

        while (_lastBunch._expires <= time)
        {
          _count -= _lastBunch._count;
          _sum = Add(_lastBunch._sum, value);
          _currentBunch._max = _bunches.Max(x => x._max);
          _currentBunch._min = _bunches.Min(x => x._min);

          _lastBunch = _bunches.Dequeue();
        }
      }
    }

    public void Reset()
    {
      _lastRefresh = _clock.Time;

      _bunches = new Queue<Bunch>();
      _currentBunch = new Bunch();
      _lastBunch = new Bunch();

      _last = default(TValue);
      _sum = default(TAggregate);
      _count = 0;
      _max = default(TValue);
      _min = default(TValue);
    }
    #endregion

    #region Abstract Declarations
    protected abstract TAggregate Add(TAggregate a, TValue b);
    protected abstract TAggregate Subtract(TAggregate a, TValue b);
    protected abstract TAggregate Divide(TAggregate a, long b);
    protected abstract TValue CalculateMin(TValue a, TValue b);
    protected abstract TValue CalculateMax(TValue a, TValue b);
    #endregion
  }
}
