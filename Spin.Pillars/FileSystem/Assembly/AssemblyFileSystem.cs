using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.Assembly
{
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

    public override Directory GetDirectory(FilePath path) => new AssemblyDirectory(this, path);
    public override File GetFile(FilePath path) => new AssemblyFile(this, path);

    public override bool FileExists(FilePath path) => FileIndex.Contains(path.ToString(PathSeparator));
    public override bool DirectoryExists(FilePath path) => DirectoryIndex.Contains(path.ToString(PathSeparator));

    public override void DeleteFile(FilePath path) => throw new NotSupportedException();
    public override void DeleteDirectory(FilePath path) => throw new NotSupportedException();
    public override void CreateFile(FilePath path) => throw new NotSupportedException();
    public override void CreateDirectory(FilePath path) => throw new NotSupportedException();

    public override IEnumerable<FilePath> GetFiles(FilePath directory)
    {
      var dir = directory.ToString(PathSeparator) + PathSeparator;
      return _files.Where(x=>x.StartsWith(dir)).Select(x => FilePath.Parse(x, PathSeparator));
    }

    public override IEnumerable<FilePath> GetDirectories(FilePath directory)
    {
      var dir = directory.ToString(PathSeparator) + PathSeparator;
      return _directories.Where(x => x.StartsWith(dir)).Select(x => FilePath.Parse(x, PathSeparator));
    }
  }
}
