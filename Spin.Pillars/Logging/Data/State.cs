using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging
{
  public class State
  {
    public string Name { get; }
    public string Value { get; }
    public bool? Active { get; }
    public string ValueText => Active.HasValue ? Active.Value ? "Active" : "Inactive" : Value;

    public State(string name, string value) => (Name, Value) = (name, value);
    public State(string name, bool active) => (Name, Active) = (name, active);

    public override string ToString() => $"{Name}: {ValueText}";
  }
}
