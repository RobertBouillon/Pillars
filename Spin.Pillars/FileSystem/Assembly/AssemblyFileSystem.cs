using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.Assembly;

public class AssemblyFileSystem : FileSystem
{
  public override TimeStamp[] SupportedDateStamps => Array.Empty<TimeStamp>();

  private string[] _files;            //This is how the source API returns the data
  private List<String> _directories;  //List is more efficient than [] because Array requires an additional resize.
  private HashSet<string> _fileIndex;
  private HashSet<string> _directoryIndex;
  internal string[] Files => _files ??= Assembly.GetManifestResourceNames();
  internal List<string> Directories => _directories ??= Files.Where(x => x.Contains(PathSeparator)).Select(io.Path.GetPathRoot).Distinct().ToList();
  internal HashSet<String> FileIndex => _fileIndex ??= Files.ToHashSet();
  internal HashSet<String> DirectoryIndex => _directoryIndex ??= Directories.ToHashSet();
  public override char PathSeparator => '\\';
  public override bool IsReadOnly => true;

  public System.Reflection.Assembly Assembly { get; }

  public AssemblyFileSystem(System.Reflection.Assembly assembly) : base(assembly.FullName) => Assembly = assembly;

  public override Directory GetDirectory(Path path) => new AssemblyDirectory(this, path);
  public override File GetFile(Path path) => new AssemblyFile(this, path);

  public override bool FileExists(Path path) => FileIndex.Contains(path.ToString(PathSeparator));
  public override bool DirectoryExists(Path path) => DirectoryIndex.Contains(path.ToString(PathSeparator));

  public override void DeleteFile(Path path) => throw new NotSupportedException();
  public override void DeleteDirectory(Path path, bool recurse = false) => throw new NotSupportedException();
  public override void CreateFile(Path path) => throw new NotSupportedException();
  public override void CreateDirectory(Path path) => throw new NotSupportedException();

  public override IEnumerable<Path> GetFiles(Path directory)
  {
    var dir = directory.ToString(PathSeparator) + PathSeparator;
    return _files.Where(x=>x.StartsWith(dir)).Select(x => Path.Parse(x, PathSeparator));
  }

  public override IEnumerable<Path> GetDirectories(Path directory)
  {
    var dir = directory.ToString(PathSeparator) + PathSeparator;
    return _directories.Where(x => x.StartsWith(dir)).Select(x => Path.Parse(x, PathSeparator));
  }
}
