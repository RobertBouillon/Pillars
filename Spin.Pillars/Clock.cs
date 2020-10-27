using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class Clock : IClock
  {
    #region Fields
    private Stopwatch _timer;
    private DateTime _rootTime;
    #endregion

    #region Properties
    public DateTime Time
    {
      get { return _rootTime.Add(_timer.Elapsed); }
      set
      {
        var oldtime = Time;
        var delta = oldtime - value;
        _rootTime += delta;
        OnTimeChanged(oldtime, value, delta);
      }
    }
    #endregion

    #region Constructors
    public Clock() : this(DateTime.Now)
    {

    }

    public Clock(DateTime initialTime)
    {
      #region Validation
      if (initialTime == null)
        throw new ArgumentNullException("initialTime");
      if(initialTime==DateTime.MinValue||initialTime==DateTime.MaxValue)
        throw new ArgumentException("DateTime is not valid.", "initialTime");
      #endregion

      _rootTime = initialTime;
      _timer = new Stopwatch();
      _timer.Start();
    }
    #endregion

    #region Methods
    public TimeSpan Measure(Action action) => _timer.Time(action);
    #endregion

    #region Events
    public event EventHandler<TimeChangedEventArgs> TimeChanged;
    protected void OnTimeChanged(DateTime oldTime, DateTime newTime, TimeSpan delta) => OnTimeChanged(new TimeChangedEventArgs(oldTime, newTime, delta));
    protected virtual void OnTimeChanged(TimeChangedEventArgs e) => TimeChanged?.Invoke(this, e);
    #endregion
  }
}
