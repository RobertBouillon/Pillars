using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.v1
{
  public interface ITimeMeter : IAggregate<TimeSpan, TimeSpan>
  {
    void Measure(Action action);
  }
}
