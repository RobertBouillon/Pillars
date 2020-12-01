using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TValue = System.Single;
using TAggregate = System.Decimal;
using Spin.Pillars.v1;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class SlidingFloatDecimalMeter : SlidingMeter<TValue, TAggregate>
  {
    public SlidingFloatDecimalMeter(string name, IClock clock, TimeSpan span) : base(name, clock, span) { }
    public SlidingFloatDecimalMeter(string name, IClock clock, TimeSpan span, TimeSpan refreshRate) : base(name, clock, span, refreshRate) { }

    protected override TAggregate Add(TAggregate a, TValue b) => a + (TAggregate)b;
    protected override TAggregate Subtract(TAggregate a, TValue b) => a - (TAggregate)b;
    protected override TAggregate Divide(TAggregate a, long b) => a / b;
    protected override TValue CalculateMin(TValue a, TValue b) => Math.Min(a, b);
    protected override TValue CalculateMax(TValue a, TValue b) => Math.Max(a, b);
  }
}
