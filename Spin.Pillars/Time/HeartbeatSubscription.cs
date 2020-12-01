using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Time
{
  public class HeartbeatSubscription
  {
    #region Fields
    private readonly Heartbeat _heartbeat;
    private readonly TimeSpan? _repeat;
    private DateTime? _nextCall;
    private Func<DateTime, Boolean> _filter;
    #endregion
    #region Properties
    public Heartbeat Heartbeat => _heartbeat;
    public TimeSpan? Repeat => _repeat;
    public DateTime? NextCall => _nextCall;
    public Action Callback { get; set; }
    #endregion

    #region Constructors
    public HeartbeatSubscription(Heartbeat heartbeat, DateTime start, Action callback, TimeSpan? repeatInterval, Func<DateTime, Boolean> filter)
    {
      #region Validation
      if (heartbeat == null)
        throw new ArgumentNullException(nameof(heartbeat));
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));
      #endregion
      _heartbeat = heartbeat;
      _nextCall = start;
      Callback = callback;
      _repeat = repeatInterval;
      _filter = filter;
    }
    #endregion

    #region Methods
    public DateTime? Invoke()
    {
      _nextCall = CalculateNextCall(_nextCall.Value);
      Callback();
      return _nextCall;
    }

    public void Cancel() => _heartbeat.Unsubscribe(this);
    private DateTime? CalculateNextCall(DateTime lastCall)
    {
      if (!_repeat.HasValue)
        return null;

      if (_filter == null)
        return lastCall + _repeat.Value;

      do
      {
        lastCall += _repeat.Value;
      } while (_filter == null || _filter(lastCall));
      return lastCall;
    }
    #endregion
  }
}
