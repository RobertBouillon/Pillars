using System;
using System.Collections.Generic;
using System.Collections.Hierarchy;
using System.Modules.Configuration;
using System.Modules.Logging;
using System.Modules.v1_0;
//using System.Modules.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class Module : TreeNode<IModule,IModules>, IModule
  {
    #region Fields
    private IRuntime _runtime;

    private IConfiguration _configuration;
    private ILog _log;
    private IMeters _meters;
    private IAlerts _alerts;
    #endregion

    #region Properties

    public IAlerts Alerts
    {
      get { return _alerts; }
    }

    public IRuntime Runtime
    {
      get { return _runtime; }
    }

    public IMeters Meters
    {
      get { return _meters; }
      set { _meters = value; }
    }

    public IConfiguration Configuration
    {
      get { return _configuration; }
      set { _configuration = value; }
    }

    public ILog Log
    {
      get { return _log; }
      set { _log = value; }
    }
    #endregion
    #region Overrides
    public new Module Parent
    {
      get { return (Module)base.Parent; }
      set { base.Parent = value; }
    }
    #endregion

    #region Constructors
    public Module(IRuntime runtime, string name) : base(name, new Modules(runtime))
    {
      _runtime = runtime;
      _meters = new Meters.Meters(this);
      Initialize();
    }

    public Module(IRuntime runtime, string name, Module parent)
      : base(name, parent, new Modules(runtime))
    {
      _runtime = runtime;
      _meters = new Meters.Meters(this);
      Initialize();
    }
    #endregion

    #region Methods
    public IModule AddChild(string name) => Children.Add(name);
    public new IEnumerable<IModule> Traverse() => base.Traverse().Cast<IModule>();

    private void Initialize()
    {
      _log = new Log(this);
      _alerts = new Alerts.Alerts();

      Children.Added += Children_Added;
      Children.Removed += Children_Removed;
    }

    void Children_Removed(object sender, TreeNodeEventArgs e)
    {
      var module = e.Node as Module;
      OnChildRemoved(this, module);

      module.ChildRemoved -= module_ChildRemoved;
      module.ChildAdded -= module_ChildAdded;
    }

    void Children_Added(object sender, TreeNodeEventArgs e)
    {
      var module = e.Node as Module;
      OnChildAdded(this, module);

      module.ChildRemoved += module_ChildRemoved;
      module.ChildAdded += module_ChildAdded;
    }

    void module_ChildRemoved(object sender, ChildModuleEventArgs e) => OnChildRemoved(e);
    void module_ChildAdded(object sender, ChildModuleEventArgs e) => OnChildAdded(e);
    #endregion

    #region Events
    public event EventHandler<ChildModuleEventArgs> ChildAdded;
    public event EventHandler<ChildModuleEventArgs> ChildRemoved;

    protected void OnChildAdded(IModule parent, IModule child)
    {
      OnChildAdded(new ChildModuleEventArgs(parent, child));
    }

    protected virtual void OnChildAdded(ChildModuleEventArgs e)
    {
      if (ChildAdded != null)
        ChildAdded(this, e);
    }

    protected void OnChildRemoved(IModule parent, IModule child)
    {
      OnChildRemoved(new ChildModuleEventArgs(parent, child));
    }

    protected virtual void OnChildRemoved(ChildModuleEventArgs e)
    {
      if (ChildRemoved != null)
        ChildRemoved(this, e);
    }
    #endregion
    #region Overrides
    public override string ToString() => Name;
    #endregion




  }
}
