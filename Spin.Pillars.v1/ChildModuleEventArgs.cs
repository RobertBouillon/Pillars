using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public class ChildModuleEventArgs : EventArgs
  {
    #region Fields
    private readonly IModule _parent;
    private readonly IModule _child;
    #endregion
    #region Properties
    public IModule Parent
    {
      get { return _parent; }
    }

    public IModule Child
    {
      get { return _child; }
    }
    #endregion
    #region Constructors
    public ChildModuleEventArgs(IModule parent, IModule child)
    {
      #region Validation
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (child == null)
        throw new ArgumentNullException("child");
      #endregion
      _parent = parent;
      _child = child;
    }
    #endregion
  }
}
