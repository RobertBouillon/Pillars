using System;
namespace System.Modules.v1_0
{
  public interface ILogEntry
  {
    IModule Module { get; set; }
    
    string FormatText { get; set; }
    object[] Arguments { get; set; }
    //object Data { get; set; }
    DateTime EntryTime { get; set; }
    IExceptionInfo Error { get; set; }
    LogSeverity Severity { get; set; }
    string ToString();
    string GetFormattedString();
  }
}
