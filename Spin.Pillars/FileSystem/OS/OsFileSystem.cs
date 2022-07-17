using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsFileSystem : FileSystem
  {
    public static TempFile CreateTempFile() => new TempFile(new OsFileSystem().GetFile(io.Path.GetTempFileName()));

    private static TimeStamp[] _supportedDateStamps = new TimeStamp[] { TimeStamp.Created, TimeStamp.LastAccess, TimeStamp.LastWrite };

    public override char PathSeparator => io.Path.DirectorySeparatorChar;

    public override TimeStamp[] SupportedDateStamps => _supportedDateStamps;

    static OsFileSystem() { }
    public OsFileSystem() : base(Environment.OSVersion.Platform.ToString()) { }

    public override Directory GetDirectory(Path path) => new OsDirectory(this, path);
    public override File GetFile(Path path) => new OsFile(this, path);

    public override bool FileExists(Path path) => io.File.Exists(GetPathedName(path));
    public override bool DirectoryExists(Path path) => io.Directory.Exists(GetPathedName(path));

    public override void DeleteFile(Path path) => io.File.Delete(GetPathedName(path));
    public override void DeleteDirectory(Path path) => io.Directory.Delete(GetPathedName(path));

    public override void CreateFile(Path path) => io.File.Create(GetPathedName(path)).Close();
    public override void CreateDirectory(Path path) => io.Directory.CreateDirectory(GetPathedName(path));

    public override Path ParsePath(string path) => WindowsFilePath.Parse(path);

    public override IEnumerable<Path> GetFiles(Path directory) => io.Directory.GetFiles(GetPathedName(directory)).Select(x => WindowsFilePath.Parse(x));
    public override IEnumerable<Path> GetDirectories(Path directory) => io.Directory.GetDirectories(GetPathedName(directory)).Select(x => WindowsFilePath.Parse(x));
  }
}
