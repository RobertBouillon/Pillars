using System;
using System.Collections.Generic;
using io=System.IO;
using System.Linq;
using System.Text;
using Spin.Pillars.Hierarchy;
using System.Reflection;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsDirectory : Directory
  {
    public static OsDirectory CurrentWorking => Parse(io.Directory.GetCurrentDirectory());
    public static OsDirectory CurrentExecuting => OsFile.CurrentExecuting.Directory;

    public static OsDirectory Create(io.DirectoryInfo directory) => Parse(directory.FullName);
    public static OsDirectory Parse(string path)
    {
      var root = System.IO.Path.GetPathRoot(path);
      if (!OsFileSystem.Mounts.TryGetValue(root.ToUpper(), out var fileSystem))
        throw new ArgumentException($"Unable to find an instance of the drive '{root}'");

      return new OsDirectory(fileSystem, Path.Parse(path.Substring(root.Length), fileSystem.PathSeparator));
    }

    public override OsFileSystem FileSystem => base.FileSystem as OsFileSystem;
    public override OsDirectory Parent => Path.Count == 0 ? null : new OsDirectory(FileSystem, Path.MoveUp());
    public OsDirectory(OsFileSystem fileSystem, Path path) : base(fileSystem, path) { }
  }
}
