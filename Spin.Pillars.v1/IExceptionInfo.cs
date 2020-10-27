using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System.Modules.v1_0
{
  public interface IExceptionInfo
  {
    string Message { get; }
    IExceptionInfo InnerException { get; }
    string StackTrace { get; }
    string VerboseInfo { get; }
    IDictionary Data {get;}
    string source { get; }
  }
}
