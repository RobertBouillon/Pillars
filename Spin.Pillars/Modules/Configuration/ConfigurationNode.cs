using System;
using System.Collections.Generic;

namespace Spin.Pillars.Configuration
{
  public class Configuration 
  {
    #region Fields
    private List<ConfigurationAttributes> _attributes = new List<ConfigurationAttributes>();
    #endregion

    #region Properties
    public List<ConfigurationAttributes> Attributes
    {
      get { return _attributes; }
      set { _attributes = value; }
    }

    public Dictionary<string, string> Properties
    {
      get { throw new NotImplementedException(); }
    }

    #endregion

    #region Constructos
    public Configuration()
    {
    }
    #endregion

    #region Methods
    public virtual void Reconfigure()
    {
      foreach (var attributes in _attributes)
        attributes.Apply();
      OnPropertiesChanged();
    }


    public void GetAttributes<T>() where T: ConfigurationAttributes
    {

    }
    #endregion

    #region Events

    #endregion

  }
}
