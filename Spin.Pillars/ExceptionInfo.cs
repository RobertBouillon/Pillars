using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;
using System.Collections;

namespace System.Modules
{
  public class ExceptionInfo : IExceptionInfo
  {
    #region Fields
    private string _message;
    private IExceptionInfo _innerException;
    private string _stackTrace;
    private string _verboseInfo;
    private IDictionary _data;
    private string _source;
    #endregion

    #region Properties
    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }

    public IExceptionInfo InnerException
    {
      get { return _innerException; }
      set { _innerException = value; }
    }

    public string StackTrace
    {
      get { return _stackTrace; }
      set { _stackTrace = value; }
    }

    public string VerboseInfo
    {
      get { return _verboseInfo; }
      set { _verboseInfo = value; }
    }

    public IDictionary Data
    {
      get { return _data; }
      set { _data = value; }
    }

    public string source
    {
      get { return _source; }
      set { _source = value; }
    }
    #endregion

    #region Constructors
    public ExceptionInfo()
    {
      
    }

    public ExceptionInfo(Exception ex)
    {
      #region Validation
      if (ex == null)
        throw new ArgumentNullException("ex");
      #endregion
      _message = ex.Message;
      if (ex.InnerException != null)
        _innerException = new ExceptionInfo(ex.InnerException);
      _stackTrace = ex.StackTrace;
      _verboseInfo = ex.ToString();
      _data = ex.Data;
      _source = ex.Source;
    }
    #endregion
  }
}
