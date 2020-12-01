using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public interface IFlow : IMeter
  {
    TimeSpan Precision { get; }
    TimeSpan Span { get; }
  }
}
