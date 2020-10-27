using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.Configuration
{
  public class ConfigurationAttribute
  {
    #region Fields
    private Uri _scope;
    private string _name;
    private object _value;
    #endregion

    #region Properties
    public Uri Scope
    {
      get { return _scope; }
      set { _scope = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public object Value
    {
      get { return _value; }
      set { _value = value; }
    }
    #endregion

    #region Constructors
    public ConfigurationAttribute(Uri scope, string name, object value)
    {
      _scope = scope;
      _name = name;
      _value = value;
    }
    #endregion


  }
}
