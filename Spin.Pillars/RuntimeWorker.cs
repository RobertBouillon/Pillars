using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Workers;

namespace System.Modules
{
  class RuntimeWorker : TimedWorker
  {
    #region Fields
    private readonly Runtime _runtime;
    #endregion

    private const int _bufferSize = 4096;
    List<ILogEntry> _entryBuffer = new List<ILogEntry>(_bufferSize);

    public RuntimeWorker(Runtime runtime) : base("Runtime Worker", TimeSpan.FromMilliseconds(100))
    {
      #region Validation
      if (runtime == null)
        throw new ArgumentNullException("runtime");
      #endregion
      _runtime = runtime;
    }

    protected override void Work() => Flush();

    private void Flush()
    {
      _entryBuffer.Clear();
      _entryBuffer.AddRange(_runtime.LogBuffer.TakeAll());
      if (_entryBuffer.Count == 0)
        return;

      foreach (var watcher in _runtime.LogWatchers.CopyOf())
      {
        try
        {
          //Ensure that an exception in one watcher doesn't break them all.
          watcher.Process(_entryBuffer);
        }
        catch (Exception ex)
        {
          OnError(ex);
        }
      }
    }

    protected override void OnStopped(EventArgs e)
    {
      Flush();
      base.OnStopped(e);
    }
  }
}
