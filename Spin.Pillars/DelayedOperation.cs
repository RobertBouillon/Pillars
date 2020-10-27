using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Text;

namespace System.Modules
{
  public class DelayedOperation : IScheduledOperation
  {
    public TimeSpan ExecutionTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? EffectiveStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? EffectiveEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IHeartbeat Heartbeat => throw new NotImplementedException();

    public DateTime? NextCall => throw new NotImplementedException();

    public void Cancel()
    {
      throw new NotImplementedException();
    }

    public DateTime? Invoke()
    {
      throw new NotImplementedException();
    }
  }
}
