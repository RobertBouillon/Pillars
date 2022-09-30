using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsFileSystem : FileSystem
  {
    private static TimeStamp[] _supportedDateStamps = new TimeStamp[] { TimeStamp.Created, TimeStamp.LastAccess, TimeStamp.LastWrite };

    public override char PathSeparator => io.Path.DirectorySeparatorChar;

    public static Dictionary<string, OsFileSystem> Mounts { get; }
    public override TimeStamp[] SupportedDateStamps => _supportedDateStamps;

    static OsFileSystem() => Mounts = io.DriveInfo.GetDrives().Select(x => new OsFileSystem(x)).ToDictionary(x => x.Name);
    public OsFileSystem(io.DriveInfo drive) : base(drive.Name) { }

    public override string GetPathedName(Path path) => io.Path.Combine(EnumerableEx.Single(Name).Concat(path.Nodes).ToArray());
    public override Path ParseAbsolutePath(string path) => new Path(path.Split(PathSeparator).Skip(1));
    public override Path ParsePath(string path, out bool isRooted) => (isRooted = (path.Length >= 1 && path[0] == PathSeparator) || (path.Length >= 2 && path[1] == ':')) ? new Path(path.Split(PathSeparator).Skip(1)) : new Path(path.Split(PathSeparator));

    public override Directory GetDirectory(Path path) => new OsDirectory(this, path);
    public override File GetFile(Path path) => new OsFile(this, path);

    public override bool FileExists(Path path) => io.File.Exists(GetPathedName(path));
    public override bool DirectoryExists(Path path) => io.Directory.Exists(GetPathedName(path));

    public override void DeleteFile(Path path) => io.File.Delete(GetPathedName(path));
    public override void DeleteDirectory(Path path, bool recurse) => io.Directory.Delete(GetPathedName(path), recurse);

    public override void CreateFile(Path path) => io.File.Create(GetPathedName(path)).Close();
    public override void CreateDirectory(Path path) => io.Directory.CreateDirectory(GetPathedName(path));

    public override IEnumerable<Path> GetFiles(Path directory) => io.Directory.GetFiles(GetPathedName(directory)).Select(x => Path.Parse(x.Substring(3), PathSeparator));
    public override IEnumerable<Path> GetDirectories(Path directory) => io.Directory.GetDirectories(GetPathedName(directory)).Select(x => Path.Parse(x.Substring(3), PathSeparator));
  }
}
