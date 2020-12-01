using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Time
{
  public interface IClock
  {
    DateTime Time { get; set; }
    event EventHandler<TimeChangedEventArgs> TimeChanged;
  }
}
