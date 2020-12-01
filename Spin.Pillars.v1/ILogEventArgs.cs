using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public class ILogEventArgs : EventArgs
  {
    #region Fields
    private readonly ILog _log;
    #endregion
    #region Properties
    public ILog Log
    {
      get { return _log; }
    }
    #endregion
    #region Constructors
    public ILogEventArgs(ILog log)
    {
      #region Validation
      if (log == null)
        throw new ArgumentNullException("log");
      #endregion
      _log = log;
    }
    #endregion
  }
}
