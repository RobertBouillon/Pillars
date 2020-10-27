using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.FileSystem
{
  public class DirectoryPath : FileSystemPath
  {
    public DirectoryPath(IEnumerable<string> nodes) : base(nodes)
    {
    }
  }
}
