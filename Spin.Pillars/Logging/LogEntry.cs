using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Modules.v1_0;
using System.Xml;

namespace System.Modules.Logging
{
  public class LogEntry : ILogEntry
  {
    #region Fields
    private DateTime _entryTime;
    private string _formatText;
    private object[] _arguments;
    private IExceptionInfo _error;
    private IModule _module;
    private LogSeverity _severity;
    private object _data;
    #endregion
    #region Properties
    public IModule Module
    {
      get { return _module; }
      set { _module = value; }
    }

    public object Data
    {
      get { return _data; }
      set { _data = value; }
    }

    public DateTime EntryTime
    {
      get { return _entryTime; }
      set { _entryTime = value; }
    }

    public string FormatText
    {
      get { return _formatText; }
      set { _formatText = value; }
    }

    public object[] Arguments
    {
      get { return _arguments; }
      set { _arguments = value; }
    }

    public IExceptionInfo Error
    {
      get { return _error; }
      set { _error = value; }
    }

    public LogSeverity Severity
    {
      get { return _severity; }
      set { _severity = value; }
    }

    #endregion

    #region Constructors
    public LogEntry()
    {

    }
    #endregion

    #region Overrides
    public override string ToString()
    {
      return String.Format(_formatText, _arguments);
    }
    #endregion
    #region Methods
 
    #endregion

    #region Explicit Implementations for ILog


    public string GetFormattedString()
    {
      return String.Format(_formatText, _arguments);
    }
    #endregion
  }
}
