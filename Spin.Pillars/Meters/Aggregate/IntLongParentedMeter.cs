using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;

namespace System.Modules.Logging.Meters.Aggregate
{
  public class IntLongParentedMeter : IntLongMeter
  {
    #region Fields
    private readonly IAggregate<int, long> _parent;
    #endregion

    #region Constructors
    public IntLongParentedMeter(string name, IAggregate<int,long> parent)
      : base(name)
    {
      #region Validation
      if (parent == null)
        throw new ArgumentNullException("parent");
      #endregion
      _parent = parent;
    }
    #endregion

    #region Overrides
    public override void Record(int value)
    {
      _parent.Record(value);
      base.Record(value);
    }

    #endregion
  }
}
