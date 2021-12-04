using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using System.Text;
using Spin.Pillars.Hierarchy;
using System.Reflection;

namespace Spin.Pillars.FileSystem.Assembly
{
  public class AssemblyDirectory : Directory
  {
    public override AssemblyFileSystem FileSystem => base.FileSystem as AssemblyFileSystem;

    public override AssemblyDirectory ParentDirectory => Path.Count == 0 ? null : new AssemblyDirectory(FileSystem, Path.MoveUp());

    public AssemblyDirectory(AssemblyFileSystem fileSystem, string path) : this(fileSystem, Path.Parse(path, fileSystem.PathSeparator)) { }
    public AssemblyDirectory(AssemblyFileSystem fileSystem, Path path) : base(fileSystem, path) { }
  }
}
