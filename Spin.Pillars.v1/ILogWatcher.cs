using System;
using System.Collections.Generic;
namespace System.Modules.v1_0
{
  public interface ILogWatcher : IDisposable
  {
    bool IsEnabled { get; }  //Should default to true
    void Process(IEnumerable<ILogEntry> entries);
  }
}
