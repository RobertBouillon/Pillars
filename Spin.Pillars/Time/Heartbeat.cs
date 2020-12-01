using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spin.Pillars.Workers;

namespace Spin.Pillars.Time
{
  public class Heartbeat : Worker
  {
    #region Fields
    private readonly HeartbeatSubscriptions _subscriptions;
    private readonly IClock _clock;
    private object _sync;
    #endregion

    #region Properties
    public IClock Clock => _clock;
    protected override void WaitForWork()
    {
      var wait = Math.Min((int)_subscriptions.NextTick(_clock).TotalMilliseconds, (int)WaitDelay.TotalMilliseconds);
      if(wait > 0)
        Thread.Sleep(wait);
    }

    protected override bool HasWork => _subscriptions.HasDue(_clock.Time);
    #endregion

    #region Constructors
    public Heartbeat(IClock clock) : base("Heartbeat")
    {
      #region Validation
      if (clock == null)
        throw new ArgumentNullException("clock");
      #endregion
      _clock = clock;
      _sync = new object();
      _subscriptions = new HeartbeatSubscriptions();
    }
    #endregion

    #region Methods
    protected override void Work()
    {
      lock (_sync)
      {
        var exec = new List<HeartbeatSubscription>(_subscriptions.GetDue(_clock.Time));
        _subscriptions.Remove(exec);

        foreach (var sub in exec)
          Task.Run(() => sub.Invoke());

        _subscriptions.AddRange(exec.Where(x => x.NextCall.HasValue));
      }
    }

    public void Unsubscribe(HeartbeatSubscription subscription)
    {
      lock (_sync)
        if (!_subscriptions.Remove(subscription))
          throw new ArgumentException("Subscription does not belong to the collection", "subscription");
    }

    public void Subscribe(HeartbeatSubscription subscription)
    {
      lock (_sync)
        _subscriptions.Add(subscription);
    }

    public HeartbeatSubscription Subscribe(DateTime start, Action callback)
    {
      var ret = new HeartbeatSubscription(this, start, callback, null, null);
      Subscribe(ret);
      return ret;
    }

    public HeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval)
    {
      var ret = new HeartbeatSubscription(this, start, callback, repeatInterval, null);
      Subscribe(ret);
      return ret;
    }

    public HeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval, Func<DateTime, Boolean> filter)
    {
      var ret = new HeartbeatSubscription(this, start, callback, repeatInterval, filter);
      Subscribe(ret);
      return ret;
    }
    #endregion
  }
}
