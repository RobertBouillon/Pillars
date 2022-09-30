using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Spin.Pillars.Workers
{
  public abstract class AsyncWorker
  {
    #region Fields
    private Task _workerTask;
    private volatile bool _isStopping;
    private volatile bool _isStarted;
    private volatile bool _isWorking;
    private readonly string _name;
    private CancellationTokenSource _cancellationTokenSource = new();
    #endregion
    #region Properties
    public virtual string Name => _name;

    public bool IsStopping
    {
      get { return _isStopping; }
      protected set { _isStopping = value; }
    }

    public bool IsStarted
    {
      get { return _isStarted; }
      protected set { _isStarted = value; }
    }

    public bool IsWorking
    {
      get { return _isWorking; }
      protected set { _isWorking = value; }
    }

    #endregion
    #region Constructors
    protected AsyncWorker() : this("Worker") { }
    protected AsyncWorker(string name)
    {
      #region Validation
      if (name == null)
        throw new ArgumentNullException("name");
      #endregion
      _name = name;
    }
    #endregion

    #region Methods
    public bool Start()
    {
      #region Validation
      //if (_internalInterval== TimeSpan.Zero)
      //  throw new InvalidOperationException("Interval must be greater than or equal to zero");
      //if (_internalInterval.TotalMilliseconds <= 0)
      //  throw new InvalidOperationException("Interval must be a positive number");
      if (_isStarted)
        throw new InvalidOperationException("Cannot start worker: worker already started");
      #endregion
      if (!OnStarting())
        return false;

      //Set here so no one can call Start again (save us having to declare and manage _isStarting)
      _isStarted = true;

      _workerTask = Task.Run(WorkerLoop);
      _workerTask.ContinueWith(x =>
      {
        _isStarted = false;
        OnStopped();
        _isStopping = false;
      });

      OnStarted();
      return true;
    }

    public void Stop() { }

    public virtual Task StopAsync(CancellationToken ctoken = default)
    {
      #region Validation
      if (!_isStarted)
        throw new InvalidOperationException("Worker has not started.");
      if (_isStopping)
        throw new InvalidOperationException("Worker is stopping.");
      #endregion
      if (!OnStopping())
        return Task.CompletedTask;

      _cancellationTokenSource.Cancel();
      _isStopping = true;

      return _workerTask.WaitAsync(ctoken);
    }

    protected abstract Task WorkAsync(CancellationToken token);

    protected virtual async Task WorkerLoop()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      do
      {
        if (_isStopping)
          return;

        try
        {
          if (!OnWorking())
            continue;
          _isWorking = true;
          sw.Restart();
          await WorkAsync(_cancellationTokenSource.Token);
          OnWorked(sw.Elapsed);
        }
        catch (TaskCanceledException) { }
        catch (Exception ex)
        {
          OnError(ex);
        }
        finally
        {
          _isWorking = false;
        }
      } while (!_cancellationTokenSource.IsCancellationRequested);
    }
    #endregion

    #region Events
    public event CancelEventHandler Starting;
    protected bool OnStarting() => new CancelEventArgs(false).Invoke(OnStarting);
    protected virtual void OnStarting(CancelEventArgs e) => Starting?.Invoke(this, e);

    public event EventHandler Started;
    protected void OnStarted() => OnStarted(EventArgs.Empty);
    protected virtual void OnStarted(EventArgs e) => Started?.Invoke(this, e);

    public event CancelEventHandler Stopping;
    protected bool OnStopping() => new CancelEventArgs(false).Invoke(OnStopping);
    protected virtual void OnStopping(CancelEventArgs e) => Stopping?.Invoke(this, e);

    public event EventHandler Stopped;
    protected void OnStopped() => OnStopped(EventArgs.Empty);
    protected virtual void OnStopped(EventArgs e) => Stopped?.Invoke(this, e);

    #region ErrorEventArgs Subclass
    public class ErrorEventArgs : EventArgs
    {
      public Exception Exception { private set; get; }
      internal ErrorEventArgs(Exception exception)
      {
        #region Validation
        if (exception == null)
          throw new ArgumentNullException("exception");
        #endregion
        Exception = exception;
      }
    }
    #endregion

    public event EventHandler<ErrorEventArgs> Error;
    protected void OnError(Exception exception) => OnError(new ErrorEventArgs(exception));
    protected virtual void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);

    #region WorkPerformedEventArgs Subclass
    public class WorkPerformedEventArgs : EventArgs
    {
      public TimeSpan Duration { get; private set; }
      internal WorkPerformedEventArgs(TimeSpan duration)
      {
        #region Validation
        if (duration == null)
          throw new ArgumentNullException("duration");
        #endregion
        Duration = duration;
      }
    }
    #endregion

    public event EventHandler<WorkPerformedEventArgs> Worked;
    protected void OnWorked(TimeSpan duration) => OnWorked(new WorkPerformedEventArgs(duration));
    protected virtual void OnWorked(WorkPerformedEventArgs e) => Worked?.Invoke(this, e);

    public event CancelEventHandler Working;
    protected bool OnWorking() => new CancelEventArgs(false).Invoke(OnWorking);
    protected virtual void OnWorking(CancelEventArgs e) => Working?.Invoke(this, e);
    #endregion
  }
}
