using System;
using System.Collections.Generic;
namespace Spin.Pillars.v1
{
  public interface ILogWatcher : IDisposable
  {
    bool IsEnabled { get; }  //Should default to true
    void Process(IEnumerable<ILogEntry> entries);
  }
}
