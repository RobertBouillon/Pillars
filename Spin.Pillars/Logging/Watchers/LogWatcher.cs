using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;
using System.Threading;

namespace System.Modules.Logging.Watchers
{
  public abstract class LogWatcher : ILogWatcher
  {
    #region Private Fields
    private volatile bool _isEnabled = true;
    #endregion

    #region Properties
    public bool IsEnabled
    {
      get => _isEnabled;
      set => _isEnabled = value;
    }
    #endregion

    #region Constructors
    public LogWatcher() { }
    #endregion

    #region Methods
    public abstract void Process(IEnumerable<ILogEntry> entries);
    public virtual void Dispose() { }
    #endregion
  }
}
