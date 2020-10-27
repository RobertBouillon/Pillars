using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Modules.Logging.Watchers;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class Runtime : IRuntime
  {
    #region Static Declarations
    public static Runtime Current { get; set; }
    #endregion

    #region Fields
    private readonly IModules _modules;
    private readonly IModule _module;
    private readonly IClock _clock;
    private readonly IClock _timer;
    private readonly IClock _localClock;
    private readonly IHeartbeat _heartbeat;
    private readonly ICollection<ILogWatcher> _logWatchers;
    private readonly ConcurrentBag<ILogEntry> _logBuffer;

    private bool _isStarted = false;
    private RuntimeWorker _worker;
    #endregion

    #region Properties
    public IModules Modules => _modules;
    public IClock Clock => _clock;
    public IClock Timer => _timer;
    public IClock LocalClock => _localClock;
    public IHeartbeat Heartbeat => _heartbeat;
    public ICollection<ILogWatcher> LogWatchers => _logWatchers;
    public ConcurrentBag<ILogEntry> LogBuffer => _logBuffer;
    public bool IsStarted => _isStarted;
    public IModule Module => _module;
    #endregion

    #region Constructors

    public Runtime()
    {
      _logBuffer = new ConcurrentBag<ILogEntry>();
      _clock = new UtcClock();
      _timer = new Clock();
      _localClock = new SystemClock();
      _heartbeat = new Heartbeat(_clock);
      _logWatchers = new List<ILogWatcher>();

      _worker = new RuntimeWorker(this);
      _worker.Error += (x, y) =>
      {
        _module.Log.Write(y.Exception);
        Console.WriteLine(y.Exception);
      };


      _module = new Module(this, "Application Runtime");
      _modules = new Modules(this, _module);
    }
    #endregion

    #region Methods

    public virtual void Start()
    {
      #region Validation
      if (_isStarted)
        throw new InvalidOperationException("Already started");
      #endregion
      _module.Log.Write(LogSeverity.Verbose, "Starting");
      _worker.Start();
      _module.Log.Write("Started");

      _isStarted = true;
    }

    public virtual void Stop()
    {
      #region Validation
      if (!_isStarted)
        throw new InvalidOperationException("Not started");
      #endregion
      _module.Log.Write(LogSeverity.Verbose, "Stopping");
      _worker.Stop();
      _isStarted = false;
    }

    public void Append(ILogEntry entry) => _logBuffer.Add(entry);
    #endregion
  }
}
