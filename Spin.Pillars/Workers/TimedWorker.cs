//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Threading;

//namespace Spin.Pillars.Workers
//{
//  public abstract class TimedWorker : Worker
//  {
//    private Stopwatch _stopwatch = new Stopwatch();
//    private TimeSpan _interval;
//    private TimeSpan _last;
//    private CancellationTokenSource _cancellationTokenSource = new();

//    public TimeSpan Interval
//    {
//      get => _interval;
//      set
//      {
//        if (IsStarted)
//          throw new InvalidOperationException("Cannot change this property while the worker is running");
//        _interval = value;
//      }
//    }

//    public TimedWorker(TimeSpan interval) => _interval = interval;

//    public TimedWorker(string name, TimeSpan interval) : base(name)
//    {
//      #region Validation
//      if (interval.TotalMilliseconds <= 0)
//        throw new ArgumentOutOfRangeException("interval", "interval must be breater than zero");
//      #endregion
//      _interval = interval;
//    }

//    protected override void OnStopping(CancelEventArgs e)
//    {
//      base.OnStopping(e);
//      _cancellationTokenSource.Cancel();
//    }

//    protected override bool WaitForWork()
//    {
//      var timeSinceLastRun = _stopwatch.Elapsed - _last;
//      var timeToNextRun = timeSinceLastRun + _interval;

//      if (timeToNextRun < TimeSpan.Zero)
//      {
//        OnBehindSchedule(timeToNextRun);
//        timeToNextRun = TimeSpan.Zero;
//      }

//      if (!_cancellationTokenSource.Token.WaitHandle.WaitOne(timeToNextRun))
//      {
//        _last = _stopwatch.Elapsed;
//        return true;
//      }
//      else
//        return false;
//    }

//    protected override void Work() => Work(_cancellationTokenSource.Token);
//    protected virtual void Work(CancellationToken ctoken) { }

//    protected override void OnStarted(EventArgs e)
//    {
//      _last = TimeSpan.Zero;
//      _stopwatch.Restart();
//      base.OnStarted(e);
//    }

//    #region BehindScheduleEventArgs Subclass
//    public class BehindScheduleEventArgs : EventArgs
//    {
//      public TimeSpan _lag { get; private set; }
//      internal BehindScheduleEventArgs(TimeSpan lag) => _lag = lag;
//    }
//    #endregion

//    public event EventHandler<BehindScheduleEventArgs> BehindSchedule;
//    protected void OnBehindSchedule(TimeSpan lag) => OnBehindSchedule(new BehindScheduleEventArgs(lag));
//    protected virtual void OnBehindSchedule(BehindScheduleEventArgs e) => BehindSchedule?.Invoke(this, e);
//  }
//}
