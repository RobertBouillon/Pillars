using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.OS;

public class OsObserver : Observer
{
  private io.FileSystemWatcher _watcher;
  private OsFileSystem _fileSystem;
  private Queue<(string Path, DateTime Time)> _debounceBuffer = new();
  private static readonly TimeSpan DEBOUNCE_TIME = TimeSpan.FromMilliseconds(500);

  public OsObserver(OsDirectory directory, ChangeTypes changes) : base(changes)
  {
    _fileSystem = directory.FileSystem;
    _watcher = new io.FileSystemWatcher(directory.PathedName);

    if (changes.HasFlag(ChangeTypes.Create))
      _watcher.Created += (x, y) => NotifyChanged(ChangeTypes.Create, y, true);
    if (changes.HasFlag(ChangeTypes.Delete))
      _watcher.Deleted += (x, y) => NotifyChanged(ChangeTypes.Delete, y, false);
    if (changes.HasFlag(ChangeTypes.Rename))
      _watcher.Renamed += (x, y) => NotifyChanged(ChangeTypes.Rename, y, false);
    if (changes.HasFlag(ChangeTypes.Update))
      _watcher.Changed += (x, y) => NotifyChanged(ChangeTypes.Update, y, true);

    _watcher.NotifyFilter = io.NotifyFilters.LastWrite;
    _watcher.EnableRaisingEvents = true;
  }

  private void NotifyChanged(ChangeTypes change, io.FileSystemEventArgs evt, bool debounce)
  {
    //FileSystemWatcher can raise multiple events, so we have to debounce the events
    var now = DateTime.Now;
    var expiry = now - DEBOUNCE_TIME;
    var path = evt.FullPath;

    while (_debounceBuffer.Any() && _debounceBuffer.Peek().Time < expiry)
      _debounceBuffer.Dequeue();

    if (debounce)
    {
      if (_debounceBuffer.Any(x => x.Path == path))
        return;
      _debounceBuffer.Enqueue((path, now));
    }

    OnChanged(_fileSystem[path], change);
  }

  protected override void DisposeManaged()
  {
    _watcher?.Dispose();
    _watcher = null;
    base.DisposeManaged();
  }
}
