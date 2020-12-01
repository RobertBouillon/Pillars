using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class FloatDecimalParentedMeter : FloatDecimalMeter
  {
    #region Fields
    private readonly IAggregate<float, decimal> _parent;

    #endregion

    #region Constructors
    public FloatDecimalParentedMeter(string name, IAggregate<float, decimal> parent)
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
    public override void Record(float value)
    {
      _parent.Record(value);
      base.Record(value);
    }
    #endregion
  }
}
