using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public interface IBoundAggregate : IAggregate
  {
    TimeSpan Span { get; }
  }
}
