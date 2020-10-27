using System;
using System.Collections.Generic;
using System.Collections.Hierarchy;
using System.Modules.v1_0;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.Configuration
{
  public class Configuration : IConfiguration
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

    public Dictionary<string, List<IConfiguration>> Children
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


    public void GetAttributes<T>() 
      where T: ConfigurationAttributes
    {

    }
    #endregion

    #region Events

    public event EventHandler PropertiesChanged;

    protected void OnPropertiesChanged()
    {
      OnPropertiesChanged(EventArgs.Empty);
    }

    protected virtual void OnPropertiesChanged(EventArgs e)
    {
      if (PropertiesChanged != null)
        PropertiesChanged(this, e);
    }
    #endregion

  }
}
