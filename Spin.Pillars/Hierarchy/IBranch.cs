using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public interface IBranch : ILeaf
  {
    IEnumerable<ILeaf> Children { get; }
    IEnumerable<IBranch> Branches { get; }
  }
}
