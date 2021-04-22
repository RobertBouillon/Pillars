using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data
{
  public class Scope
  {
    public Scope Parent { get; set; }

    public Scope() { }
    public Scope(Scope parent) => Parent = parent;

  }
}
