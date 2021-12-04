using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  [Flags]
  public enum ChangeTypes
  {
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 4,
    Rename = 8,
    All = 
      Create +
      Update + 
      Delete +
      Rename
  }
}
