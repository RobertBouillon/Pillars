using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.v1_0
{
  public class ILogEntryEventArgs : EventArgs
  {
    #region Fields
    private readonly ILogEntry _logEntry;
    #endregion
    #region Properties
    public ILogEntry LogEntry
    {
      get { return _logEntry; }
    }
    #endregion
    #region Constructors
    public ILogEntryEventArgs(ILogEntry logEntry)
    {
      #region Validation
      if (logEntry == null)
        throw new ArgumentNullException("logEntry");
      #endregion
      _logEntry = logEntry;
    }
    #endregion
  }
}
