using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;

namespace System.Modules.Logging.Watchers
{
  public class LogWatchers : CollectionBase<ILogWatcher>, ILogWatchers
  {

    public LogWatchers()
    {

    }

    public LogWatchers(IEnumerable<LogWatcher> source) : base(source)
    {

    }

    public void Start()
    {
      foreach (var watcher in this)
        if (!watcher.IsRunning)
          watcher.Start();
    }

    public void Stop()
    {
      foreach (var watcher in this)
        if (watcher.IsRunning)
          watcher.Stop();
    }
  }
}
