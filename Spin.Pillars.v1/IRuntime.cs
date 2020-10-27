using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IRuntime
  {
    IModules Modules { get; }
    IModule Module { get; }
    ICollection<ILogWatcher> LogWatchers { get; }
    IHeartbeat Heartbeat { get; }
    IClock Clock { get; }   //UTC Clock. May Change
    IClock Timer { get; }   //Guaranteed not to change
    IClock LocalClock { get; }   //Localized time.

    void Append(ILogEntry entry);  //Records a log entry
  }
}
