using System;
using System.Collections.Generic;
using System.Modules.Meters;
using System.Linq;
using System.Text;

namespace System.Modules.Logging
{
  public class LongCounter : Counter<long>
  {
    public LongCounter(string name)
      : base(name)
    {

    }

    protected override long Aggregate(long a, long b)
    {
      return a + b;
    }

    protected override long Divide(long a, long b)
    {
      return a / b;
    }
  }
}
