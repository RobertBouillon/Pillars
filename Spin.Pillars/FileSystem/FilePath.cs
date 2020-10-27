using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.FileSystem
{
  public class FilePath : FileSystemPath
  {
    public FilePath(IEnumerable<string> nodes) : base(nodes) { }
    public FilePath(string path, char separator) : base(path, separator) { }
    public FilePath(string path, string separator) : base(path, separator) { }
  }
}
