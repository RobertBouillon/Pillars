using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.Configuration
{
  public abstract class ConfigurationAttributes : CollectionBase<ConfigurationAttribute>
  {
    public virtual Uri Scope
    {
      get
      {
        return GetUriFromType(GetType());
      }
    }


    public ConfigurationAttributes()
    {

    }

    public abstract void Apply();
    public abstract void Export();

    public ConfigurationAttribute this[string name]
    {
      get
      {
        return this.First(x=>x.Scope == GetUriFromType(GetType()) && x.Name == name);
      }
    }

    public ConfigurationAttribute this[Uri scope, string name]
    {
      get
      {
        return this.FirstOrDefault(x => x.Scope == scope && x.Name == name);
      }
    }

    public static Uri GetUriFromType(Type type)
    {
      return new Uri("uri://" + type.FullName.Replace('.','/'));
    }

    public void Add(string name)
    {
      Add(name, null);
    }

    public void Add(string name, object defaultValue)
    {
      Add(new ConfigurationAttribute(GetUriFromType(GetType()), name, defaultValue));
    }

    public virtual object Construct()
    {
      throw new NotImplementedException();
    }
  }
}
