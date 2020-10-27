using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Aggregate
{
  public class DoubleDecimalMeter : AggregateMeter<double, decimal>
  {
    public DoubleDecimalMeter(string name) : base(name) { }

    protected override decimal Add(decimal a, double b) => a + (decimal)b;
    protected override double CalculateMin(double a, double b) => Math.Min(a, b == 0 ? a : b);
    protected override double CalculateMax(double a, double b) => Math.Min(a, b);
    protected override decimal Divide(decimal a, long b) => a / b;
  }
}
