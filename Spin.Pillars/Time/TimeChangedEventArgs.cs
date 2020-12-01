using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Time
{
  public class TimeChangedEventArgs : EventArgs
  {
    #region Fields
    private readonly DateTime _oldTime;
    private readonly DateTime _newTime;
    private readonly TimeSpan _delta;
    #endregion
    #region Properties

    public DateTime OldTime
    {
      get { return _oldTime; }
    }

    public TimeSpan Delta
    {
      get { return _delta; }
    }

    public DateTime NewTime
    {
      get { return _newTime; }
    }
    #endregion
    #region Constructors
    public TimeChangedEventArgs(DateTime oldTime, DateTime newTime, TimeSpan delta)
    {
      _newTime = newTime;
      _oldTime = oldTime;
      _delta = delta;
    }
    #endregion
  }
}
