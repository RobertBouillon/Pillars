using System;
using System.Collections;

namespace Spin.Pillars.Logging;

public class ExceptionInfo
{
  #region Properties
  public string Message { get; set; }
  public ExceptionInfo InnerException { get; set; }
  public string StackTrace { get; set; }
  public string VerboseInfo { get; set; }
  public IDictionary Data { get; set; }
  public string Source { get; set; }
  #endregion

  #region Constructors
  public ExceptionInfo() { }

  public ExceptionInfo(Exception ex)
  {
    #region Validation
    if (ex == null)
      throw new ArgumentNullException("ex");
    #endregion
    Message = ex.Message;
    if (ex.InnerException != null)
      InnerException = new ExceptionInfo(ex.InnerException);
    StackTrace = ex.StackTrace;
    VerboseInfo = ex.ToString();
    Data = ex.Data;
    Source = ex.Source;
  }
  #endregion
}
