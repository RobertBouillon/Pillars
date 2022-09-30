//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Spin.Pillars.Workers
//{
//  public class ThreadedWorker : Worker
//  {
//    #region Fields
//    private Thread _workerThread;
//    private volatile bool _isStopping;
//    private volatile bool _isStarted;
//    private volatile bool _isWorking;
//    private readonly string _name;
//    #endregion
//    #region Properties
//    //protected abstract WaitHandle WorkHandle { get; }

//    protected Thread WorkerThread => _workerThread;

//    public virtual string Name => _name;

//    public bool IsStopping
//    {
//      get { return _isStopping; }
//      protected set { _isStopping = value; }
//    }

//    public bool IsStarted
//    {
//      get { return _isStarted; }
//      protected set { _isStarted = value; }
//    }

//    public bool IsWorking
//    {
//      get { return _isWorking; }
//      protected set { _isWorking = value; }
//    }

//    #endregion
//    #region Constructors
//    protected Worker() : this("Worker") { }
//    protected Worker(string name)
//    {
//      #region Validation
//      if (name == null)
//        throw new ArgumentNullException("name");
//      #endregion
//      _name = name;
//    }
//    #endregion

//    #region Methods
//    protected virtual Thread CreateThread(String name)
//    {
//      Thread worker = new Thread(WorkerLoop);
//      if (!String.IsNullOrWhiteSpace(name))
//        worker.Name = _name;
//      worker.Priority = ThreadPriority.Normal;
//      worker.IsBackground = true;
//      worker.Start();
//      return worker;
//    }

//    public bool Start()
//    {
//      #region Validation
//      //if (_internalInterval== TimeSpan.Zero)
//      //  throw new InvalidOperationException("Interval must be greater than or equal to zero");
//      //if (_internalInterval.TotalMilliseconds <= 0)
//      //  throw new InvalidOperationException("Interval must be a positive number");
//      if (_isStarted)
//        throw new InvalidOperationException("Cannot start worker: worker already started");
//      #endregion
//      if (!OnStarting())
//        return false;

//      //Set here so no one can call Start again (save us having to declare and manage _isStarting)
//      _isStarted = true;

//      _workerThread = CreateThread(_name);

//      OnStarted();
//      return true;
//    }

//    public bool Stop() => Stop(TimeSpan.FromSeconds(3));
//    public virtual bool Stop(TimeSpan timeout)
//    {
//      #region Validation
//      if (!_isStarted)
//        throw new InvalidOperationException("Worker has not started.");
//      if (_isStopping)
//        throw new InvalidOperationException("Worker is stopping.");
//      #endregion
//      if (!OnStopping())
//        return false;

//      _isStopping = true;
//      try
//      {
//        _isStarted = false;
//        OnStopped();
//      }
//      finally
//      {
//        _isStopping = false;
//      }

//      return !_isStarted;
//    }

//    protected abstract void Work();

//    protected virtual void WorkerLoop()
//    {
//      Stopwatch sw = new Stopwatch();
//      sw.Start();
//      do
//      {
//        WaitForWork();

//        if (_isStopping)
//          return;

//        try
//        {
//          if (!OnWorking())
//            continue;
//          _isWorking = true;
//          sw.Restart();
//          Work();
//          OnWorked(sw.Elapsed);
//        }
//        catch (Exception ex)
//        {
//          OnError(ex);
//        }
//        finally
//        {
//          _isWorking = false;
//        }
//      } while (true);
//    }

//    //True if there's work to be done
//    protected abstract bool WaitForWork();

//    #endregion

//    #region Events
//    public event CancelEventHandler Starting;
//    protected bool OnStarting() => new CancelEventArgs(false).Invoke(OnStarting);
//    protected virtual void OnStarting(CancelEventArgs e) => Starting?.Invoke(this, e);

//    public event EventHandler Started;
//    protected void OnStarted() => OnStarted(EventArgs.Empty);
//    protected virtual void OnStarted(EventArgs e) => Started?.Invoke(this, e);

//    public event CancelEventHandler Stopping;
//    protected bool OnStopping() => new CancelEventArgs(false).Invoke(OnStopping);
//    protected virtual void OnStopping(CancelEventArgs e) => Stopping?.Invoke(this, e);

//    public event EventHandler Stopped;
//    protected void OnStopped() => OnStopped(EventArgs.Empty);
//    protected virtual void OnStopped(EventArgs e) => Stopped?.Invoke(this, e);

//    #region ErrorEventArgs Subclass
//    public class ErrorEventArgs : EventArgs
//    {
//      public Exception Exception { private set; get; }
//      internal ErrorEventArgs(Exception exception)
//      {
//        #region Validation
//        if (exception == null)
//          throw new ArgumentNullException("exception");
//        #endregion
//        Exception = exception;
//      }
//    }
//    #endregion

//    public event EventHandler<ErrorEventArgs> Error;
//    protected void OnError(Exception exception) => OnError(new ErrorEventArgs(exception));
//    protected virtual void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);

//    #region WorkPerformedEventArgs Subclass
//    public class WorkPerformedEventArgs : EventArgs
//    {
//      public TimeSpan Duration { get; private set; }
//      internal WorkPerformedEventArgs(TimeSpan duration)
//      {
//        #region Validation
//        if (duration == null)
//          throw new ArgumentNullException("duration");
//        #endregion
//        Duration = duration;
//      }
//    }
//    #endregion

//    public event EventHandler<WorkPerformedEventArgs> Worked;
//    protected void OnWorked(TimeSpan duration) => OnWorked(new WorkPerformedEventArgs(duration));
//    protected virtual void OnWorked(WorkPerformedEventArgs e) => Worked?.Invoke(this, e);

//    public event CancelEventHandler Working;
//    protected bool OnWorking() => new CancelEventArgs(false).Invoke(OnWorking);
//    protected virtual void OnWorking(CancelEventArgs e) => Working?.Invoke(this, e);
//    #endregion
//  }
//}
