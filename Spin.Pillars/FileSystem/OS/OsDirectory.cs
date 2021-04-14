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
    public static OsDirectory CurrentWorking => new OsDirectory(io.Directory.GetCurrentDirectory());
    public static OsDirectory CurrentExecuting => OsFile.CurrentExecuting.Directory;

    public OsProvider OsProvider => Provider as OsProvider;

    public override OsDirectory Parent => Path.Count == 0 ? null : new OsDirectory(OsProvider, Path.MoveUp());

    public OsDirectory(io.DirectoryInfo directory) : this(directory.FullName) { }
    public OsDirectory(string path)
    {
      var root = System.IO.Path.GetPathRoot(path);
      if (!OsProvider.Mounts.TryGetValue(root.ToUpper(), out var provider))
        throw new ArgumentException($"Unable to find an instance of the drive '{root}'");

      Provider = provider;
      Path = new Path(path.Substring(root.Length), Provider.PathSeparator);
    }

    public OsDirectory(OsProvider provider, Path path)
    {
      #region Validation
      if (provider is null)
        throw new ArgumentNullException(nameof(provider));
      #endregion
      Provider = provider;
      Path = path;
    }

    public override IEnumerable<File> GetFiles() => new System.IO.DirectoryInfo(Provider.GetFullPath(Path)).GetFiles().Select(x => new OsFile(OsProvider, OsProvider.ParseAbsolutePath(x.FullName)));
    public override IEnumerable<Directory> GetDirectories() => new System.IO.DirectoryInfo(Provider.GetFullPath(Path)).GetDirectories().Select(x => new OsDirectory(OsProvider, OsProvider.ParseAbsolutePath(x.FullName)));

    public override void Create() => io.Directory.CreateDirectory(FullName);
    public override bool Exists() => io.Directory.Exists(FullName);

    public override void MoveTo(Directory destination, bool overwrite = true, bool recurse = false)
    {
      if (destination is OsDirectory dir)
        System.IO.Directory.Move(Provider.GetFullPath(Path), Provider.GetFullPath(dir.Path));
      else
        throw new NotSupportedException();
    }

    public override void CopyTo(Directory destination, bool overwrite = true, bool recurse = false)
    {
      foreach (var file in GetFiles())
        file.CopyTo(destination, overwrite);

      if (recurse)
        foreach (var directory in GetDirectories())
          directory.CopyTo(destination.Create(directory.Name), overwrite, true);
    }
  }
}
