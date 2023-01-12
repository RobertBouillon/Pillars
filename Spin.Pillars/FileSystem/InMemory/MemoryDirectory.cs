using Spin.Pillars.Hierarchy;
using System;
using io = System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.FileSystem.InMemory;

internal class MemoryDirectory
{
  private string _name;

  public string Name
  {
    get => _name;
    set
    {
      if (_name == value)
        return;

      if (Parent is null)
      {
        _name = value;
      }
      else
      {
        Parent.Directories.Remove(_name);
        _name = value;
        Parent.Directories.Add(_name, this);
      }
    }
  }

  public Dictionary<string, MemoryFile> Files { get; set; } = new Dictionary<string, MemoryFile>();
  public Dictionary<string, MemoryDirectory> Directories { get; set; } = new Dictionary<string, MemoryDirectory>();
  public MemoryDirectory Parent { get; set; }
  public Path Path => Parent is null ? new Path(EnumerableEx.Single(Name)) : new Path(Parent.Traverse(x => x.Parent).Where(x => x.Parent is not null).Reverse().Select(x => x.Name).Concat(Name).ToArray());

  internal MemoryDirectory(string name) => _name = name;
  public MemoryDirectory(MemoryDirectory parent, string name)
  {
    Parent = parent;
    _name = name;
  }

  internal void CreateFile(string name) => Files.Add(name, new MemoryFile(this, name));
  internal void CreateDirectory(string name) => Directories.Add(name, new MemoryDirectory(this, name));
  public override string ToString() => Path.ToString('\\');
}
