using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public interface IBranch : ILeaf
  {
    IEnumerable<ILeaf> Children { get; }
    IEnumerable<IBranch> Branches => Children.OfType<IBranch>();
  }
}
