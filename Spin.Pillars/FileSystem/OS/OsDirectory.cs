using System;
using io=System.IO;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsDirectory : Directory, IObservable
  {
    public static OsDirectory CurrentWorking => Parse(io.Directory.GetCurrentDirectory());
    public static OsDirectory CurrentExecuting => OsFile.CurrentExecuting.ParentDirectory;
    public static OsDirectory Temp => Parse(io.Path.GetTempPath());

    public static OsDirectory Create(io.DirectoryInfo directory) => Parse(directory.FullName);
    public static OsDirectory Parse(string path) => new OsDirectory(new OsFileSystem(), WindowsFilePath.Parse(path));

    public Observer Observe(ChangeTypes types, string filter = null) => new OsObserver(this, types);
    public override OsFileSystem FileSystem => base.FileSystem as OsFileSystem;
    public override OsDirectory ParentDirectory => Path.Count == 0 ? null : new OsDirectory(FileSystem, Path.MoveUp());
    public OsDirectory(OsFileSystem fileSystem, Path path) : base(fileSystem, path) { }
  }
}
