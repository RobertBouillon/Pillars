using System;
using System.Collections.Generic;
using System.Linq;
using Spin.Pillars.v1;
using System.Text;
using System.Threading.Tasks;
using FlowType = System.Int64;

namespace Spin.Pillars.Meters.Flow
{
  public class LongFlow : Flow<FlowType>
  {
    #region Constructors
    public LongFlow(string name, IMeter<FlowType> source, TimeSpan span, TimeSpan precision, IHeartbeat heartbeat)
      : base(name, source, span, precision, heartbeat)
    {

    }
    #endregion
    #region Overrides
    protected override FlowType Add(FlowType a, FlowType b)
    {
      return a + b;
    }

    protected override FlowType Subtract(FlowType a, FlowType b)
    {
      return a - b;
    }
    #endregion
  }
}
