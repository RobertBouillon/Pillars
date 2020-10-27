﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TValue = System.Single;
using TAggregate = System.Double;
using System.Modules.v1_0;

namespace System.Modules.Logging.Meters.Aggregate
{
  class SlidingFloatDoubleMeter : SlidingMeter<TValue, TAggregate>
  {
    public SlidingFloatDoubleMeter(string name, IClock clock, TimeSpan span) : base(name, clock, span) { }
    public SlidingFloatDoubleMeter(string name, IClock clock, TimeSpan span, TimeSpan refreshRate) : base(name, clock, span, refreshRate) { }

    protected override TAggregate Add(TAggregate a, TValue b) => a + b;
    protected override TAggregate Subtract(TAggregate a, TValue b) => a - b;
    protected override TAggregate Divide(TAggregate a, long b) => a / b;
    protected override TValue CalculateMin(TValue a, TValue b) => Math.Min(a, b);
    protected override TValue CalculateMax(TValue a, TValue b) => Math.Max(a, b);
  }
}
