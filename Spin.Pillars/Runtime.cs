using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.Logging;
using Spin.Pillars.Logging.Writers;
using Spin.Pillars.Time;

namespace Spin.Pillars
{
  public class Runtime
  {
    public static Runtime Current { get; set; }

    private readonly Module _module;
    private readonly UtcClock _clock;
    private readonly Clock _timer;
    private readonly SystemClock _localClock;
    private readonly Heartbeat _heartbeat;
    private readonly LogWriter _logWriter;
    private readonly ConcurrentBag<LogEntry> _logBuffer;

    private bool _isStarted = false;

    public IEnumerable<Module> Modules => Module.Traverse(x => x.Children);
    public UtcClock Clock => _clock;
    public Clock Timer => _timer;
    public SystemClock SystemClock => _localClock;
    public Heartbeat Heartbeat => _heartbeat;
    public LogWriter LogWriter => _logWriter;
    public ConcurrentBag<LogEntry> LogBuffer => _logBuffer;
    public bool IsStarted => _isStarted;
    public Module Module => _module;

    public Runtime(string applicationName, LogWriter writer)
    {
      _logBuffer = new ConcurrentBag<LogEntry>();
      _clock = new UtcClock();
      _timer = new Clock();
      _localClock = new SystemClock();
      _heartbeat = new Heartbeat(_clock);  //Not started, by default. May not be needed.
      _logWriter = writer;

      _module = new Module(applicationName, _logWriter, _clock);
    }
  }
}
