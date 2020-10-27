using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Modules.v1_0;
using System.Modules.Meters;
using System.Modules.Logging.Watchers;
using System.Threading;

namespace System.Modules.Logging
{
  public class Log : ILog
  {
    #region Fields
    private IModule _module;
    #endregion
    #region Properties
    public IModule Module => _module;
    #endregion
    #region Constructors
    internal Log(IModule module)
    {
      #region Validation
      if (module == null)
        throw new ArgumentNullException(nameof(module));
      #endregion
      _module = module;
    }
    #endregion
    #region Methods
    public ILogEntry Write(string format, params object[] args) => Write(LogSeverity.Trace, format, args);
    public ILogEntry Write(Exception ex, string text, params object[] args) => Write(LogSeverity.Error, ex, text, args);
    public ILogEntry Write(Exception ex) => Write(ex, ex.Message);

    public void Write(ILogEntry log) => _module.Runtime.Append(log);

    public ILogEntry Write(LogSeverity severity, string format, params object[] args)
    {
      var ret = new LogEntry
      {
        EntryTime = _module.Runtime.Clock.Time,
        Module = _module,
        FormatText = format,
        Arguments = args,
        Severity = severity
      };

      Write(ret);
      return ret;
    }

    public ILogEntry Write(LogSeverity severity, Exception ex, string text, params object[] args)
    {
      var ret = new LogEntry
      {
        EntryTime = _module.Runtime.Clock.Time,
        FormatText = text,
        Arguments = args,
        Severity = severity,
        Error = new ExceptionInfo(ex),
        Module = _module
      };

      Write(ret);
      return ret;
    }
    #endregion

    #region Overrides
    public override string ToString() => _module.FullPath;

    #endregion
  }
}
