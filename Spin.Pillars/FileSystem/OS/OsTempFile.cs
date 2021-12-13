using Spin.Pillars.Hierarchy;
using System;
using io=System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsTempFile : OsFile
  {
    public static new OsTempFile Create()
    {
      var path = io.Path.GetTempFileName();
      var root = System.IO.Path.GetPathRoot(path);
      if (!OsFileSystem.Mounts.TryGetValue(root, out var fileSystem))
        throw new ArgumentException($"Unable to find an instance of the drive '{root}'");

      return new OsTempFile(fileSystem, Path.Parse(path.Substring(root.Length), fileSystem.PathSeparator));
    }

    ~OsTempFile() => Dispose(false);

    private OsTempFile(OsFileSystem fileSystem, Path path) : base(fileSystem, path) { }
    protected override void DisposeNative()
    {
      Delete();
    }
  }
}
