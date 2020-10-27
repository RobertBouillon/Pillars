using System;
using System.Collections.Generic;
using System.Collections.Hierarchy;
using System.Modules.v1_0;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class Modules : TreeNodes<IModule>, IModules
  {
    #region Fields
    private IRuntime _runtime;
    #endregion

    #region Properties

    #endregion

    #region Constructors
    public Modules(IRuntime runtime)
    {
      _runtime = runtime;
    }

    public Modules(IRuntime runtime, params IModule[] source)
      : this(runtime, (IEnumerable<IModule>)source)
    {
    }

    public Modules(IRuntime runtime, IEnumerable<IModule> source)
      : base(source)
    {
      _runtime = runtime;
    }
    #endregion

    #region IComponents Members


    public override IModule Add(string name)
    {
      var ret = new Module(_runtime, name);
      base.Add(ret);
      return ret;
    }


    public override IModule this[string name]
    {
      get
      {
        if (name.Contains(PathDelimiter))
        {
          int i = name.IndexOf(PathDelimiter);
          var shortname = name.Substring(0, i);
          var child = name.Substring(i + 1, name.Length - i - 1);
          var ret = this.FirstOrDefault<IModule>(x => x.Name == shortname);
          return (ret == null) ? Add(shortname).Children[child] : ret[child];
        }
        else
        {
          var ret = this.FirstOrDefault<IModule>(x => x.Name == name);
          if (ret == null)
            ret = Add(name);
          return ret;
        }
      }
    }

    public new IModule this[int index]
    {
      get
      {
        return (IModule) base[index];
      }
      set
      {
        base[index] = (ITreeNode) value;
      }
    }
    #endregion

  }
}
