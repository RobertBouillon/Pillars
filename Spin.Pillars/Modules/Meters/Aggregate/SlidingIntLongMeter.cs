using System;
using System.Collections.Generic;
using Spin.Pillars.v1;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TValue = System.Int32;
using TAggregate = System.Int64;


namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class SlidingIntLongMeter : SlidingMeter<TValue, TAggregate>
  {
    public SlidingIntLongMeter(string name, IClock clock, TimeSpan span) : base(name, clock, span) { }
    public SlidingIntLongMeter(string name, IClock clock, TimeSpan span, TimeSpan refreshRate) : base(name, clock, span, refreshRate) { }

    protected override TAggregate Add(TAggregate a, TValue b) => a + b;
    protected override TAggregate Subtract(TAggregate a, TValue b) => a - b;
    protected override TAggregate Divide(TAggregate a, TAggregate b) => a / b;
    protected override TValue CalculateMin(TValue a, TValue b) => Math.Min(a, b);
    protected override TValue CalculateMax(TValue a, TValue b) => Math.Max(a, b);
  }
}
