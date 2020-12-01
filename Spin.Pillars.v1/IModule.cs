using System;
using System.Collections.Generic;
using System.Collections.Hierarchy;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public interface IModule : ITreeNode<IModule, IModules>, ITreeNode
  {
    IRuntime Runtime { get; }
    ILog Log { get; set; }
    IConfiguration Configuration { get; set; }
    new IEnumerable<IModule> Traverse();
    IModule AddChild(string name);
    new IModules Children { get; }

    IMeters Meters { get; }
    IAlerts Alerts { get; }

    event EventHandler<ChildModuleEventArgs> ChildAdded;
    event EventHandler<ChildModuleEventArgs> ChildRemoved;
  }
}
