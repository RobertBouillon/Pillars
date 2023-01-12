using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Spin.Pillars.Workers;

public abstract class Worker
{
  public bool IsStopping { get; protected set; }
  public bool IsStarted { get; protected set; }
  public bool IsFaulted { get; protected set; }

  protected abstract void DoStart();
  protected abstract void DoStop();

  public virtual bool Start()
  {
    #region Validation
    if (IsStarted)
      throw new InvalidOperationException("Cannot start worker: worker already started");
    #endregion
    if (!OnStarting())
      return false;

    DoStart();

    IsStarted = true;

    OnStarted();
    return true;
  }

  public bool Stop() => Stop(TimeSpan.FromSeconds(3));
  public virtual bool Stop(TimeSpan timeout)
  {
    #region Validation
    if (!IsStarted)
      throw new InvalidOperationException("Worker has not started.");
    if (IsStopping)
      throw new InvalidOperationException("Worker is stopping.");
    #endregion
    if (!OnStopping())
      return false;

    IsStopping = true;
    try
    {
      DoStop();
      IsStarted = false;
      OnStopped();
    }
    finally
    {
      IsStopping = false;
    }

    return !IsStarted;
  }

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

  //Only raised when an error occurrs in the background work - not on the public sync methods
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
