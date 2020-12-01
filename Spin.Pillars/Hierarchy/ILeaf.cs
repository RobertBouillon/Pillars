using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public interface ILeaf
  {
    Path Path => new Path(this);
    IBranch Parent { get; }
    string Name { get; }
  }
}
