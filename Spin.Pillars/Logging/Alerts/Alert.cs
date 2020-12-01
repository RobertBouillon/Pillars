using System;
using System.Collections.Generic;
using System.Linq;
using Spin.Pillars.v1;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Alerts
{
  public class Alert : IAlert
  {
    #region Fields
    private readonly TimeSpan _cooldownPeriod;
    private readonly IModule _source;
    private readonly FormattedString _message;

    private DateTime _lastTriggered;
    private ulong _triggerCount;
    private bool _isActive;
    private bool _isVisible;
    private bool _isSupressed;
    private readonly AlertType _type;

    #endregion

    #region Properties

    public AlertType Type
    {
      get { return _type; }
    }
    
    /// <summary>
    /// The time between LastTriggered and now is less than the cooldown period
    /// </summary>
    public bool IsActive
    {
      get { return _isActive; }
      set { _isActive = value; }
    }

    /// <summary>
    /// Is shown to the user.
    /// </summary>
    public bool IsVisible
    {
      get { return _isVisible; }
      set { _isVisible = value; }
    }

    /// <summary>
    /// Is not visible when activated
    /// </summary>
    public bool IsSupressed
    {
      get { return _isSupressed; }
      set { _isSupressed = value; }
    }

    #endregion

    #region Constructors
    public Alert(IModule source,  AlertType type, TimeSpan cooldown, string message, params object[] arguments)
    {
      _source = source;
      _cooldownPeriod = cooldown;
      _message = new FormattedString(message, arguments);
    }
	  #endregion


    public string Message
    {
      get { return _message.ToString(); }
    }

    public TimeSpan ExpirationTimeout
    {
      get { return _cooldownPeriod; }
    }

    public DateTime LastTriggered
    {
      get { return _lastTriggered; }
    }

    public ulong TriggerCount
    {
      get { return _triggerCount; }
    }

    public void Trigger()
    {
      _lastTriggered = _source.Runtime.Clock.Time;
      ++_triggerCount;

      if(!_isActive)
      {
        _isActive = true;
        if (!_isSupressed)
          _isVisible = true;
      }
    }
  }
}
