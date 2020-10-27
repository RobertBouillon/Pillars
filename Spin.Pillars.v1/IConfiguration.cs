using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.v1_0
{
  public interface IConfiguration
  {
    Dictionary<string, string> Properties { get; }
    Dictionary<string, List<IConfiguration>> Children { get; }

    event EventHandler PropertiesChanged;
  }
}
