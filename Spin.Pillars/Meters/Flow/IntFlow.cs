using System;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;
using FlowType = System.Int32;

namespace System.Modules.Meters.Flow
{
  public class IntFlow : Flow<FlowType>
  {
    #region Constructors
    public IntFlow(string name, IMeter<FlowType> source, TimeSpan span, TimeSpan precision,IHeartbeat heartbeat)
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
