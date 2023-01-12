using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem;

public interface IEntity : ILeaf
{
  FileSystem FileSystem { get; }
  string PathedName { get; }
  Directory ParentDirectory { get; }
}
