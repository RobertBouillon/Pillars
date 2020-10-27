using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IHeartbeat
  {
    IClock Clock { get; }
    bool IsStarted { get; }

    void Start();
    void Stop();

    void Subscribe(IHeartbeatSubscription subscription);
    void Unsubscribe(IHeartbeatSubscription subscription);

    IHeartbeatSubscription Subscribe(DateTime start, Action callback);
    IHeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval);
    IHeartbeatSubscription Subscribe(DateTime start, Action callback, TimeSpan repeatInterval, Func<DateTime, Boolean> filter);
  }
}
