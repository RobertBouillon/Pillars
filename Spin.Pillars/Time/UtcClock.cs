using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Time
{
  public class UtcClock : IClock
  {
    public UtcClock() { }

    public DateTime Time
    {
      get
      {
        return DateTime.UtcNow;
      }

      set
      {
        throw new NotSupportedException();
      }
    }

    public event EventHandler<TimeChangedEventArgs> TimeChanged;
  }
}
