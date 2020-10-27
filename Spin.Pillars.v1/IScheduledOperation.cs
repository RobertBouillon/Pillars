using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IScheduledOperation : IHeartbeatSubscription
  {
    TimeSpan ExecutionTime { get; set; }      //The time to execute the operation
    DateTime? EffectiveStart { get; set; }    //Only execute after this date.
    DateTime? EffectiveEnd { get; set; }      //Only execute before this date
    
    bool IsEnabled { get; set; }
  }
}
