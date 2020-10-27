using System;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  class HeartbeatSubscriptions : SortedSet<IHeartbeatSubscription>
  {
    private class SubscriptionComparer : Comparer<IHeartbeatSubscription>
    {
      public override int Compare(IHeartbeatSubscription x, IHeartbeatSubscription y)
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

    public void Reset(IHeartbeatSubscription subscription) 
    {

      #region Validation
      if (subscription == null)
        throw new ArgumentNullException("subscription");
      #endregion
      Remove(subscription);
      Add(subscription);
    }

    public bool HasDue(DateTime now) => NextTick() < now;

    public IEnumerable<IHeartbeatSubscription> GetDue(DateTime now)
    {
      for (var e = GetEnumerator(); e.Current.NextCall <= now; e.MoveNext())
        yield return e.Current;
    }

    public void AddRange(IEnumerable<IHeartbeatSubscription> subs)
    {
      foreach (var sub in subs)
        Add(sub);
    }
  }
}
