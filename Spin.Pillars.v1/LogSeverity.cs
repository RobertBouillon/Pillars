using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public enum LogSeverity : byte
  {
    /// <summary>
    /// A trace used only during run-time debugging. Verbose traces should be disabled during normal operation
    /// </summary>
    Verbose = 0,

    /// <summary>
    /// A normal event has occurred that should be logged.
    /// </summary>
    Trace = 1,

    /// <summary>
    /// An abnormal event has occurred that should be logged, but lacks the severity to notify an administrator
    /// </summary>
    Notification = 2,

    /// <summary>
    /// An abnormal event has occurred and requires administrative attention
    /// </summary>
    Alert = 3,

    /// <summary>
    /// An application error has occurred and requires administrative attention.
    /// </summary>
    Error = 4,

    /// <summary>
    /// A critical system error has occurred and requires immediate administrative attention.
    /// </summary>
    FatalError = 5
  }
}
