using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spin.Pillars.Hierarchy;
using io = System.IO;
using System.IO;

namespace Spin.Pillars.FileSystem.InMemory
{
  internal class MemoryFile
  {
    public io.MemoryStream Stream { get; }
    private string _name;
    public MemoryDirectory Directory { get; set; }

    public string Name
    {
      get => _name;
      set
      {
        if (_name == value)
          return;
        Directory.Files.Remove(_name);
        _name = value;
        Directory.Files.Add(_name, this);
      }
    }

    public FileSize Size { get; }
    public bool IsReadOnly { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime CreationTime { get; set; }
    public Hierarchy.Path Path => new(Directory.Traverse(x => x.Parent).Where(x => x.Parent is not null).Reverse().Select(x => x.Name).Concat(Name).ToArray());

    public MemoryFile(MemoryDirectory directory, string name)
    {
      Directory = directory;
      _name = name;
      Stream = new MemoryStream();
    }

    private MemoryFile(string name, io.MemoryStream stream, MemoryDirectory parent)
    {
      _name = name;
      Stream = stream;
      Directory = parent;
    }

    public void Delete() => Directory.Files.Remove(_name);

    public MemoryFile Clone(MemoryDirectory dir = null) => new MemoryFile(_name, Stream.Clone(), dir ?? Directory);
  }
}
