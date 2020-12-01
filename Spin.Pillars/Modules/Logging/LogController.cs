using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Modules.v1_0;
using System.Text.RegularExpressions;

using System.Modules.Logging.Watchers;

namespace System.Modules.Logging
{
  public class LogController : ILogController
  {
    #region Static Declarations
    private static LogController _current = new LogController();
    public static LogController Current { get { return _current; } }
    #endregion

    #region Fields
    private LogWatchers _logWatchers; //Global log watchers
    private Logs _allLogs;
    private Logs _rootLogs;
    private ILog _log;

    private Thread _logProcessor;
    private bool _isLogProcessorStopping = false;
    private bool _isLogProcessorRunning;

    private LogWatchers _allWatchers;
    private LogWatchers _globalWatchers;

    private volatile Mutex _sync;
    private TimeSpan _safeLockTimeout = TimeSpan.FromSeconds(0.5);
    private ConsoleLogWatcher _consoleLogWatcher;
    private EventHandler<ILogEventArgs> _consoleLogWatcherAddHandler;
    private EventHandler<ILogEventArgs> _consoleLogWatcherRemoveHandler;
    private IClock _clock;
    #endregion

    #region Properties

    public IClock Clock
    {
      get { return _clock; }
      set { _clock = value; }
    }


    public Logs AllLogs
    {
      get { return _allLogs; }
      set { _allLogs = value; }
    }

    public bool LogToConsole
    {
      get { return _consoleLogWatcher != null; }
      set
      {
        if (LogToConsole == value)
          return;
        if (value)
        {
          _consoleLogWatcher = new ConsoleLogWatcher(_allLogs);
          _globalWatchers.Add(_consoleLogWatcher);
          _consoleLogWatcher.Start();
        }
        else
        {
          _consoleLogWatcher.Stop();
          _globalWatchers.Remove(_consoleLogWatcher);
          _consoleLogWatcher = null;
        }

      }
    }
    public LogWatchers AllWatchers
    {
      get { return _allWatchers; }
    }

    public LogWatchers GlobalWatchers
    {
      get { return _globalWatchers; }
    }

    public bool IsLogProcessorRunning
    {
      get { return _isLogProcessorRunning; }
    }
    #endregion

    #region Constructors
    public LogController()
    {
      _allLogs = new Logs();
      _rootLogs = new Logs();
      _allWatchers = new LogWatchers();
      _globalWatchers = new LogWatchers();

      _sync = new Mutex();

      _allLogs.Inserted += new EventHandler<CollectionBase<ILog>.InsertedEventArgs>(_allLogs_Inserted);
      _allLogs.Removed += new EventHandler<CollectionBase<ILog>.RemovedEventArgs>(_allLogs_Removed);

      _globalWatchers.Inserting += new EventHandler<CollectionBase<ILogWatcher>.InsertingEventArgs>(_globalWatchers_Inserting);
      _globalWatchers.Inserted += (x, y) => _allWatchers.Add(y.Item);

      SecureCollection(_allLogs);
      SecureCollection(_rootLogs);
      SecureCollection(_allWatchers);
      SecureCollection(_globalWatchers);

      _clock = new Clock();
      _log = GetLog("Log Controller");
    }

    void _globalWatchers_Inserting(object sender, CollectionBase<ILogWatcher>.InsertingEventArgs e)
    {
      if (e.Item.Recurse)
        e.CancelException = new Exception("Cannot add watch to global watches that is set to recursive");
    }

    void _allLogs_Removed(object sender, CollectionBase<ILog>.RemovedEventArgs e)
    {
      foreach (var watcher in _globalWatchers)
        watcher.UnmonitorLog(e.Item);
    }

    void _allLogs_Inserted(object sender, CollectionBase<ILog>.InsertedEventArgs e)
    {
      SafeLock(() =>
      {
        foreach (var watcher in _globalWatchers)
          watcher.MonitorLog(e.Item);
      });
    }

    private void SecureCollection<T>(CollectionBase<T> target)
    {
      target.Inserting += (x, y) => AcquireSafeLock();
      target.Removing += (x, y) => AcquireSafeLock();
      target.Inserted += (x, y) => _sync.ReleaseMutex();
      target.Removed += (x, y) => _sync.ReleaseMutex();
    }

    #endregion


    #region Methods

    public IMeter FindMetric(string fullName)
    {
      Regex logname = new Regex(@"(.+\\?)\\(.+)");
      Match m = logname.Match(fullName);
      return GetLog(m.Groups[1].Value).Meters[m.Groups[2].Value];
    }

    public void FlushWatchers()
    {
      ProcessWatchers();

      foreach (var watcher in _allWatchers)
        watcher.Flush();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    /// <remarks>This is the ONLY place a log should be allowed to be created, for the sake of global log handlers</remarks>
    public ILog GetLog(string fullName)
    {
      Queue<String> names = new Queue<string>(fullName.Split('\\'));
      string root = names.Dequeue();
      string name;
      ILog log = _rootLogs[root] ?? CreateLog(root);

      if (names.Count > 0)
        do
          log = log[name = names.Dequeue()] ?? CreateLog(name, log);
        while (names.Count > 0);

      return log;
    }

    internal Log CreateLog(string name, ILog parent = null)
    {
      Log log = new Log(this, name, parent);
      OnLogAdded(log);
      _allLogs.Add(log);

      if (parent == null)
        _rootLogs.Add(log);
      //else  //Handled in log constructor
      //  parent.Children.Add(log);

      return log;
    }

    private ILog FindLog(string fullName)
    {
      ILog ret;
      Queue<String> names = new Queue<string>(fullName.Split('\\'));
      for(ret = _rootLogs[names.Dequeue()];ret!=null;ret = ret.Children[names.Dequeue()]);
      
      return ret;
    }

    public void StartLogProcessor()
    {
      #region Validation
      if(_isLogProcessorRunning)
        throw new InvalidOperationException("Log processor already running");
      #endregion
      _logProcessor = new Thread((x) => WorkerLoop());
      _logProcessor.Priority = ThreadPriority.BelowNormal;
      _logProcessor.Name = "Log Processor";
      _logProcessor.IsBackground = true;

      _isLogProcessorStopping = false;

      _logProcessor.Start();
      _isLogProcessorRunning = true;
    }

    private void WorkerLoop()
    {
      while (!_isLogProcessorStopping)
      {
        ProcessLogs();
        ProcessWatchers();
        Thread.Sleep(100);  //TODO: Make this smarter... should only sleep if there's NOTHING to do.
      }
      SafeLock(() =>
      {
        foreach (var watcher in _allWatchers)
          watcher.Flush();
      });
    }

    public void LogHighError(string format, params object[] args)
    {
      OnHighErrorLogged(String.Format(format, args));
    }

    private void SafeLock(Action action)
    {
      if (!_sync.WaitOne(_safeLockTimeout))
        LogHighError("Lock timeout exceeded: deadlock possible.");
      else
      {
        try
        {
          action();
        }
        finally
        {
          _sync.ReleaseMutex();
        }
      }
    }

    /// <summary>Locks the mutex with a common timeout method to prevent deadlocks</summary>
    private void AcquireSafeLock()
    {
      if (!_sync.WaitOne(_safeLockTimeout))
        LogHighError("Lock timeout exceeded: deadlock possible.");
    }

    public void ProcessWatchers()
    {
      SafeLock(() =>
      {
        foreach (var watcher in _allWatchers)
          watcher.Monitor();
      });
    }

    public void StopLogProcessor()
    {
      #region Validation
      if (_isLogProcessorRunning)
        throw new InvalidOperationException("Log processor already running");
      #endregion
      _isLogProcessorStopping = true;
      if(!_logProcessor.Join(TimeSpan.FromSeconds(2)))
      {
        _logProcessor.Abort();
        if (!_logProcessor.Join(TimeSpan.FromSeconds(2)))
          throw new Exception("Unable to stop log processor thread");
      }
      _isLogProcessorRunning = false;
    }

    //Processes logs on the calling thread
    public void ProcessLogs()
    {
      SafeLock(() =>
      {
        foreach (Log log in _allLogs)
          log.Monitor();
      });
    }
    #endregion

    #region Events



    public event EventHandler<ILogEntryEventArgs> LogEntryAdded;

    protected void OnLogEntryAdded(ILogEntry logEntry)
    {
      OnLogEntryAdded(new ILogEntryEventArgs(logEntry));
    }

    protected virtual void OnLogEntryAdded(ILogEntryEventArgs e)
    {
      if (LogEntryAdded != null)
        LogEntryAdded(this, e);
    }




    public event EventHandler<ILogEventArgs> LogAdded;

    protected void OnLogAdded(ILog log)
    {
      OnLogAdded(new ILogEventArgs(log));
    }

    protected virtual void OnLogAdded(ILogEventArgs e)
    {
      if (LogAdded != null)
        LogAdded(this, e);
    }



    #region HighErrorLoggedEventArgs Subclass
    public class HighErrorLoggedEventArgs : EventArgs
    {
      #region Fields
      private readonly string _error;
      #endregion
      #region Properties
      public string Error
      {
        get { return _error; }
      }
      #endregion
      #region Constructors
      internal HighErrorLoggedEventArgs(string error)
      {
        #region Validation
        if (error == null)
          throw new ArgumentNullException("error");
        #endregion
        _error = error;
      }
      #endregion
    }
    #endregion

    public event EventHandler<HighErrorLoggedEventArgs> HighErrorLogged;

    protected void OnHighErrorLogged(string error)
    {
      OnHighErrorLogged(new HighErrorLoggedEventArgs(error));
    }

    protected virtual void OnHighErrorLogged(HighErrorLoggedEventArgs e)
    {
      if (HighErrorLogged != null)
        HighErrorLogged(this, e);
    }



    #endregion

    #region Explicit ILogController Implementations
    IList<ILog> ILogController.AllLogs
    {
      get
      {
        return this.AllLogs;
      }
    }

    IList<ILogWatcher> ILogController.GlobalWatchers
    {
      get
      {
        return (IList<ILogWatcher>) this.GlobalWatchers;
      }
    }

    IList<ILogWatcher> ILogController.AllWatchers
    {
      get
      {
        return (IList<ILogWatcher>) this.AllWatchers;
      }
    }
    #endregion


    //#region Unit Test Methods
    //[UnitTest(TestMethodType.CreateInstance)]
    //private static LogController CreateTest()
    //{
    //  LogController lc = new LogController();
    //  lc.StartLogProcessor();
    //  return lc;
    //}

    //[UnitTest(TestMethodType.DestroyInstance)]
    //private static void DestroyTest(LogController lc)
    //{
    //  lc.StopLogProcessor();
    //}
    //#endregion

  }
}
