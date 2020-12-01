using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.Time
{
  public class SystemClock : IClock
  {
    public DateTime Time { get => DateTime.Now; set => throw new NotImplementedException(); }
    public event EventHandler<TimeChangedEventArgs> TimeChanged;
  }
}
