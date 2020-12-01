using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public interface IAlert
  {
    bool IsActive { get; set; }
    bool IsVisible { get; set; }
    bool IsSupressed { get; set; }
    string Message { get; }
    TimeSpan ExpirationTimeout { get; }
    DateTime LastTriggered { get; }
    ulong TriggerCount { get; }

    void Trigger();
  }
}
