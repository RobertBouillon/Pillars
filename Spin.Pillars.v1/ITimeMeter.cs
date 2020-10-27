using System;
using System.Collections.Generic;
using System.Text;

namespace System.Modules.v1_0
{
  public interface ITimeMeter : IAggregate<TimeSpan, TimeSpan>
  {
    void Measure(Action action);
  }
}
