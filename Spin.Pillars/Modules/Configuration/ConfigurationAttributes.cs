using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Configuration
{
  public abstract class ConfigurationAttributes : CollectionBase<ConfigurationAttribute>
  {
    public virtual Uri Scope => GetUriFromType(GetType());
    
    public ConfigurationAttributes() { }

    public abstract void Apply();
    public abstract void Export();

    public ConfigurationAttribute this[string name] => this.First(x => x.Scope == GetUriFromType(GetType()) && x.Name == name);
    public ConfigurationAttribute this[Uri scope, string name] => this.FirstOrDefault(x => x.Scope == scope && x.Name == name);

    public static Uri GetUriFromType(Type type) => new Uri("uri://" + type.FullName.Replace('.', '/'));

    public void Add(string name) => Add(name, null);
    public void Add(string name, object defaultValue) => Add(new ConfigurationAttribute(GetUriFromType(GetType()), name, defaultValue));
    public virtual object Construct() => throw new NotImplementedException();
  }
}
