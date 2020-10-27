using System;
namespace System.Modules.v1_0
{
  public interface IHeartbeatSubscription
  {
    IHeartbeat Heartbeat { get; }

    DateTime? NextCall { get; }
    DateTime? Invoke();
    void Cancel();
  }
}
