using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data;

public class Scope
{
  public string[] Path { get; set; }

  public Scope(string[] path) => Path = path;
  public Scope(Path path) => Path = path.Nodes;

  public override string ToString() => Path.Join('\\');
}
