using System;
using System.Collections.Generic;
using Spin.Pillars.Logging;
using System.Linq;
using Spin.Pillars.Hierarchy;
using Spin.Pillars.Logging.Writers;
using Spin.Pillars.Time;

namespace Spin.Pillars
{
  public class Module : IBranch
  {
    #region Overrides
    public string Name { get; set; }
    public Module Parent { get; set; }
    public List<Module> Children { get; } = new List<Module>();
    public Path Path => new Path(this);
    public Log Log { get; set; }
    #endregion

    #region Constructors
    public Module(string name, LogWriter logWriter, IClock clock)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      if (logWriter is null)
        throw new ArgumentNullException(nameof(logWriter));
      if (clock is null)
        throw new ArgumentNullException(nameof(clock));
      #endregion

      Name = name;
      Log = new Log(this, logWriter, clock);
    }

    public Module(string name, Module parent, LogWriter logWriter = null, IClock clock = null)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      if (parent is null)
        throw new ArgumentNullException(nameof(parent));
      #endregion

      Name = name;
      Parent = parent;

      Log = new Log(this, logWriter ?? parent.Log.Writer, clock ?? parent.Log.Clock);
    }

    #endregion

    public Module AddChild(string name, LogWriter logWriter = null, IClock clock = null)
    {
      var added = new Module(name, this, logWriter, clock);
      Children.Add(added);
      return added;
    }

    #region Overrides
    public override string ToString() => Name;
    #endregion

    #region IBranch Members
    IEnumerable<ILeaf> IBranch.Children => Children;
    IBranch ILeaf.Parent => Parent;
    string ILeaf.Name => Name;
    #endregion
  }
}
