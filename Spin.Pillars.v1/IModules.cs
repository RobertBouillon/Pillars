using System;
using System.Collections.Generic;
using System.Collections.Hierarchy;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IModules : IList<IModule>, ITreeNodes<IModule>, ITreeNodes, IEnumerable<IModule>
  {
    new IModule Add(string name);
    new IModule this[string name] { get; }
  }
}
