using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class FloatDecimalMeter : AggregateMeter<float, decimal>
  {
    public FloatDecimalMeter(string name) : base(name){}

    protected override decimal Add(decimal a, float b) => a + (decimal)b;
    protected override decimal Divide(decimal a, long b) => b == 0 ? 0 : a / b;
    protected override float CalculateMin(float a, float b) => Math.Min(a, b == 0 ? a : b);
    protected override float CalculateMax(float a, float b) => Math.Max(a, b);
  }
}
