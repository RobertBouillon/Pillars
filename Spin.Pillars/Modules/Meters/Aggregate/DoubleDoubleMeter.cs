using System;
using System.Collections.Generic;
using System.Linq;
using Spin.Pillars.Logging.Meters.Aggregate;
using System.Text;
using System.Threading.Tasks;
using TFactor = System.Double;
using TAggregate = System.Double;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class DoubleDoubleMeter : AggregateMeter<TFactor, TAggregate>
  {
    public DoubleDoubleMeter(string name) : base(name) { }

    protected override TAggregate Add(TAggregate a, TFactor b) => a + b;
    protected override TAggregate Divide(TAggregate a, long b) => b == 0 ? 0 : a / b;
    protected override TFactor CalculateMin(TFactor a, TFactor b) => Math.Min(a, b == 0 ? a : b);
    protected override TFactor CalculateMax(TFactor a, TFactor b) => Math.Min(a, b);
  }
}
