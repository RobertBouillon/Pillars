using System;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Modules
{
  public class Heartbeat : Worker, IHeartbeat
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
        var exec = new List<IHeartbeatSubscription>(_subscriptions.GetDue(_clock.Time));
        _subscriptions.Remove(exec);

        foreach (var sub in exec)
          Task.Run(() => sub.Invoke());

        _subscriptions.AddRange(exec.Where(x => x.NextCall.HasValue));
      }
    }

    public void Unsubscribe(IHeartbeatSubscription subscription)
    {
      lock (_sync)
        if (!_subscriptions.Remove(subscription))
          throw new ArgumentException("Subscription does not belong to the collection", "subscription");
    }

    void IHeartbeat.Start() => Start();
    void IHeartbeat.Stop() => Stop();

    public void Subscribe(IHeartbeatSubscription subscription)
    {
      lock (_sync)
        _subscriptions.Add(subscription);
    }

    public IHeartbeatSubscription Subscribe(DateTime start, Action callback)
    {
      var ret = new TimeDelayedEvent(this, start, callback, null, null);
      Subscribe(ret);
      return ret;
    }

    public IHeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval)
    {
      var ret = new TimeDelayedEvent(this, start, callback, repeatInterval, null);
      Subscribe(ret);
      return ret;
    }

    public IHeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval, Func<DateTime, Boolean> filter)
    {
      var ret = new TimeDelayedEvent(this, start, callback, repeatInterval, filter);
      Subscribe(ret);
      return ret;
    }
    #endregion
  }
}
