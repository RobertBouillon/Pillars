using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.Logging.Meters.Aggregate
{
  public class IntLongMeter : AggregateMeter<int, long>
  {
    public IntLongMeter(string name) : base(name){}

    protected override long Add(long a, int b) => a + b;
    protected override long Divide(long a, long b) => b == 0 ? 0 : a / b;
    protected override int CalculateMin(int a, int b) => Math.Min(a, b == 0 ? a : b);
    protected override int CalculateMax(int a, int b) => Math.Max(a, b);
  }
}
