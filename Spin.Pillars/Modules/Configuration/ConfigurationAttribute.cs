using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Configuration
{
  public class ConfigurationAttribute
  {
    #region Properties
    public Uri Scope { get; set; }
    public string Name { get; set; }
    public object Value { get; set; }
    #endregion

    #region Constructors
    public ConfigurationAttribute(Uri scope, string name, object value)
    {
      Scope = scope;
      Name = name;
      Value = value;
    }
    #endregion
  }
}
