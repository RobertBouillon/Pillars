using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Modules.Logging.Meters.Aggregate;
using System.Modules.v1_0;
using System.Text;

namespace System.Modules.Meters
{
  public class TimeMeter : AggregateMeter<TimeSpan, TimeSpan>, ITimeMeter
  {
    public TimeMeter(string name) : base(name){}

    public void Measure(Action action) => Record(new Stopwatch().Time(action));
    protected override TimeSpan Add(TimeSpan a, TimeSpan b) => a + b;
    protected override TimeSpan CalculateMax(TimeSpan a, TimeSpan b) => TimeSpan.FromTicks(Math.Max(a.Ticks, b.Ticks));
    protected override TimeSpan CalculateMin(TimeSpan a, TimeSpan b) => TimeSpan.FromTicks(Math.Min(a.Ticks, b.Ticks));
    protected override TimeSpan Divide(TimeSpan a, long b) => TimeSpan.FromTicks(a.Ticks / b);
  }
}
