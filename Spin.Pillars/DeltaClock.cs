using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class DeltaClock : IClock
  {
    #region Fields
    private IClock _clock;
    private TimeSpan _delta;
    #endregion

    #region Constructors
    public DeltaClock(IClock clock)
    {
      #region Validation
      if (clock == null)
        throw new ArgumentNullException("clock");
      #endregion
      _clock = clock;
    }

    public DeltaClock(IClock clock, DateTime time) : this(clock)
    {
      _delta = clock.Time - time;
    }

    public DeltaClock(IClock clock, TimeSpan delta) : this(clock)
    {
      _delta = delta;
    }
    #endregion

    #region Properties

    public DateTime Time
    {
      get
      {
        return _clock.Time + _delta;
      }
      set
      {
        var oldtime = Time;
        var delta = oldtime - value;
        _delta = delta;
        OnTimeChanged(oldtime, value, delta);
      }
    }
    #endregion

    #region Events

    public event EventHandler<TimeChangedEventArgs> TimeChanged;

    protected void OnTimeChanged(DateTime oldTime, DateTime newTime, TimeSpan delta)
    {
      OnTimeChanged(new TimeChangedEventArgs(oldTime, newTime, delta));
    }

    protected virtual void OnTimeChanged(TimeChangedEventArgs e)
    {
      if (TimeChanged != null)
        TimeChanged(this, e);
    }
    #endregion
  }
}
