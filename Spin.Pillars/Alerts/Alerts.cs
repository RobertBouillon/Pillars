using System;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.Alerts
{
  public class Alerts : CollectionBase<IAlert>, IAlerts
  {
    #region Static Declarations
    private static readonly TimeSpan _defaultCooldown = TimeSpan.FromHours(2);
    #endregion

    #region Fields
    private readonly IModule _module;
    private Dictionary<IAlert, IAlert> _index; //TODO: Look for a better collection to handle this.
    #endregion

    #region Constructors
    public Alerts(IModule module)
    {
      #region Validation
      if (module == null)
        throw new ArgumentNullException("module");
      #endregion
      _module = module;
    }


    public Alerts()
    {

    }

    public Alerts(IEnumerable<IAlert> source ) : base(source)
    {

    }
    #endregion


    public IAlert Raise(AlertType type, TimeSpan cooldown, string message, params object[] args)
    {
      #region Validation
      if (_module == null)
        throw new InvalidOperationException("Cannot raise an alert in this context. User the overload that includes the IModule argument");
      if (message == null)
        throw new ArgumentNullException("message");
      #endregion
      Alert alert = new Alert(_module, type, cooldown, message, args);
      IAlert ret;
      if (!_index.TryGetValue(alert, out ret))
        ret = alert;

      ret.Trigger();
      return ret;
    }

    public IAlert Raise(AlertType type, string message, params object[] args)
    {
      #region Validation
      if (_module == null)
        throw new InvalidOperationException("Cannot raise an alert in this context. User the overload that includes the IModule argument");
      if (message == null)
        throw new ArgumentNullException("message");
      #endregion
      return Raise(type, _defaultCooldown, message, args);
    }
  }
}
