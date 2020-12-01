using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{
  public interface IMeter
  {
    bool Enabled { get; set; }
    string Name { get; }
    object Value { get; set; }
    void Reset();
  }
}
