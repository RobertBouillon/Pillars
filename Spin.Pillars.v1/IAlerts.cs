using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IAlerts : IList<IAlert>
  {
    IAlert Raise(AlertType type, TimeSpan cooldown, string message, params object[] args);
    IAlert Raise(AlertType type, string message, params object[] args);
  }
}
