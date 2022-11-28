using io = System.IO;
using Spin.Pillars.Hierarchy;
using System.Collections.Generic;
using System.Linq;
using System;

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

    public override OsDirectory GetDirectory(Path path) => base.GetDirectory(path) as OsDirectory;
    public override OsDirectory GetDirectory(string name) => base.GetDirectory(name) as OsDirectory;
    public override OsFile GetFile(Path path) => base.GetFile(path) as OsFile;
    public override OsFile GetFile(string name) => base.GetFile(name) as OsFile;

    public override IEnumerable<OsFile> GetFiles() => base.GetFiles().Cast<OsFile>();
    public override IEnumerable<OsDirectory> GetDirectories() => base.GetDirectories().Cast<OsDirectory>();

    public override IEnumerable<OsDirectory> FindDirectories(Func<Directory, bool> predicate = null, bool recursive = false) => base.FindDirectories(predicate, recursive).Cast<OsDirectory>();
    public override IEnumerable<OsFile> FindFiles(Func<File, bool> predicate = null, bool recursive = false) => base.FindFiles(predicate, recursive).Cast<OsFile>();

    
  }
}

