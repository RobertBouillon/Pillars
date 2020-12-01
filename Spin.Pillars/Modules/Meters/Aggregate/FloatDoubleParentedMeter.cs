using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.v1;

namespace Spin.Pillars.Logging.Meters.Aggregate
{
  public class FloatDoubleParentedMeter : FloatDoubleMeter
  {
    #region Fields
    private IAggregate<float, double> _parent;
    #endregion

    #region Constructors
    public FloatDoubleParentedMeter(string name, IAggregate<float, double> parent) : base(name)
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
