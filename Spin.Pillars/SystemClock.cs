using System;
using System.Collections.Generic;
using System.Modules.v1_0;
using System.Text;

namespace System.Modules
{
  class SystemClock : IClock
  {
    public DateTime Time { get => DateTime.Now; set => throw new NotImplementedException(); }

    public event EventHandler<TimeChangedEventArgs> TimeChanged;
  }
}
