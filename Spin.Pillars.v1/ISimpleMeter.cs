using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public interface ISimpleMeter : IMeter
  {
    string Name { get; set; }
    object Increment();
    object Decrement();
    object Increment(object value);
    object Decrement(object value);

    object Value { get; }
    void Reset();
  }
}
