using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Aggregate
{
  public class FloatDoubleMeter: AggregateMeter<float, double>
  {
    public FloatDoubleMeter(string name) : base(name){}
    protected override double Add(double a, float b) => a + b;
    protected override double Divide(double a, long b) => b == 0 ? 0 : a / b;
    protected override float CalculateMin(float a, float b) => Math.Min(a, b == 0 ? a : b);
    protected override float CalculateMax(float a, float b) => Math.Max(a, b);
  }
}
