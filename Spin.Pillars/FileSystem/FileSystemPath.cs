using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem
{
  public class FileSystemPath : Path
  {
    public FileSystemPath(IEnumerable<string> nodes) : base(nodes) { }
    public FileSystemPath(string path, char separator) : base(path, separator) { }
    public FileSystemPath(string path, string separator) : base(path, separator) { }
  }
}
