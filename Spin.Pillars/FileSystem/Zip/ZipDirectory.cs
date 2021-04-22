using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.Zip
{
  public class ZipDirectory : Directory
  {
    public override ZipFileSystem FileSystem => base.FileSystem as ZipFileSystem;
    public override ZipDirectory Parent => Path.Count == 0 ? null : new ZipDirectory(FileSystem, Path.MoveUp());

    internal ZipDirectory(ZipFileSystem fileSystem, ZipDirectory directory) : base(fileSystem, directory.Path) { }
    public ZipDirectory(ZipFileSystem fileSystem, Path path) : base(fileSystem, path) { }
  }
}