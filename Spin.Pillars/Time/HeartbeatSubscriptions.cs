using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Time
{
  class HeartbeatSubscriptions : SortedSet<HeartbeatSubscription>
  {
    private class SubscriptionComparer : Comparer<HeartbeatSubscription>
    {
      public override int Compare(HeartbeatSubscription x, HeartbeatSubscription y)
      {
        var xn = x.NextCall;
        var yn = y.NextCall;

        if (!xn.HasValue && !yn.HasValue)
          return 0;
        else if (!xn.HasValue)
          return -1;
        else if (!yn.HasValue)
          return 1;
        else
          return xn.Value.CompareTo(yn.Value);
      }
    }

    private static SubscriptionComparer _comparer = new SubscriptionComparer();
    public DateTime NextTick() => this.First().NextCall.Value;
    public TimeSpan NextTick(IClock clock) => clock.Time - NextTick();

    public HeartbeatSubscriptions() : base(_comparer){}

    public void Reset(HeartbeatSubscription subscription) 
    {
      #region Validation
      if (subscription == null)
        throw new ArgumentNullException("subscription");
      #endregion
      Remove(subscription);
      Add(subscription);
    }

    public bool HasDue(DateTime now) => NextTick() < now;

    public IEnumerable<HeartbeatSubscription> GetDue(DateTime now)
    {
      for (var e = GetEnumerator(); e.Current.NextCall <= now; e.MoveNext())
        yield return e.Current;
    }

    public void AddRange(IEnumerable<HeartbeatSubscription> subs)
    {
      foreach (var sub in subs)
        Add(sub);
    }
  }
}
