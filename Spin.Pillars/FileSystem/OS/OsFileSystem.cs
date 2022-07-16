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

    public override string GetPathedName(FilePath path) => io.Path.Combine(EnumerableEx.Single(Name).Concat(path.Nodes).ToArray());
    public override FilePath ParseAbsolutePath(string path) => new FilePath(path.Split(PathSeparator).Skip(1));

    public override Directory GetDirectory(FilePath path) => new OsDirectory(this, path);
    public override File GetFile(FilePath path) => new OsFile(this, path);

    public override bool FileExists(FilePath path) => io.File.Exists(GetPathedName(path));
    public override bool DirectoryExists(FilePath path) => io.Directory.Exists(GetPathedName(path));

    public override void DeleteFile(FilePath path) => io.File.Delete(GetPathedName(path));
    public override void DeleteDirectory(FilePath path) => io.Directory.Delete(GetPathedName(path));

    public override void CreateFile(FilePath path) => io.File.Create(GetPathedName(path)).Close();
    public override void CreateDirectory(FilePath path) => io.Directory.CreateDirectory(GetPathedName(path));

    public override IEnumerable<FilePath> GetFiles(FilePath directory) => io.Directory.GetFiles(GetPathedName(directory)).Select(x => FilePath.Parse(x.Substring(3), PathSeparator));
    public override IEnumerable<FilePath> GetDirectories(FilePath directory) => io.Directory.GetDirectories(GetPathedName(directory)).Select(x => FilePath.Parse(x.Substring(3), PathSeparator));
  }
}
