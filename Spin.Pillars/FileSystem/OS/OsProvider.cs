using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsProvider : Provider
  {
    private static TimeStamp[] _supportedDateStamps = new TimeStamp[] { TimeStamp.Created, TimeStamp.LastAccess, TimeStamp.LastWrite };

    public static Dictionary<string, OsProvider> Mounts { get; }
    public override TimeStamp[] SupportedDateStamps => _supportedDateStamps;

    static OsProvider() => Mounts = io.DriveInfo.GetDrives().Select(x => new OsProvider(x)).ToDictionary(x => x.Name);

    public OsProvider(io.DriveInfo drive) : base(drive.Name) => PathSeparator = io.Path.DirectorySeparatorChar;

    public override string GetFullPath(Path path) => io.Path.Combine(EnumerableEx.Single(Name).Concat(path.Nodes).ToArray());
    public override Path ParseAbsolutePath(string path) => new Path(path.Split(PathSeparator).Skip(1));
    public override Path ParsePath(string path, out bool isRooted) => (isRooted = (path.Length >= 1 && path[0] == PathSeparator) || (path.Length >= 2 && path[1] == ':')) ? new Path(path.Split(PathSeparator).Skip(1)) : new Path(path.Split(PathSeparator));

    public override Directory GetDirectory(Path path) => new OsDirectory(this, path);
    public override File GetFile(Path path) => new OsFile(this, path);
  }
}
