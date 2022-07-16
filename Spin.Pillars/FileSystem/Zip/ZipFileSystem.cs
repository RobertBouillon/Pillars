using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Spin.Pillars.FileSystem.Zip
{
  public class ZipFileSystem : FileSystem
  {
    private static TimeStamp[] _supportedDateStamps = new TimeStamp[] { TimeStamp.LastWrite };
    public override TimeStamp[] SupportedDateStamps => _supportedDateStamps;
    private ZipArchive _file;
    private bool _readMode;
    public override char PathSeparator => '\\';

    public override bool IsReadOnly => _readMode;

    public ZipFileSystem(File source) : base(source.PathedName) { }

    public override Directory GetDirectory(FilePath path) => new ZipDirectory(this, path);
    public override File GetFile(FilePath path) => new ZipFile(this, path);

    public override bool FileExists(string path) => _file.Entries.Any(x => x.FullName == path);
    public override bool DirectoryExists(string path) => _file.Entries.Any(x => x.FullName.StartsWith(path));

    public override bool FileExists(FilePath path) => FileExists(GetPathedName(path) + PathSeparator);
    public override bool DirectoryExists(FilePath path) => FileExists(GetPathedName(path) + PathSeparator);

    public override void DeleteFile(string path) => FindFile(path).Delete();
    public override void DeleteFile(FilePath path) => DeleteFile(GetPathedName(path));
    public override void DeleteDirectory(FilePath path)
    {
      var dir = GetPathedName(path) + PathSeparator;
      foreach (var file in _file.Entries.Where(x => x.FullName.StartsWith(dir)))
        file.Delete();
    }

    public override void CreateFile(FilePath path)
    {
      var name = GetPathedName(path);
      if (FileExists(path))
        throw new Exception($"'{name}' already exists");
      _file.CreateEntry(name);
    }
    public override void CreateDirectory(FilePath path) => throw new NotSupportedException();

    internal ZipArchiveEntry FindFile(FilePath path) => FindFile(GetPathedName(path));
    internal ZipArchiveEntry FindFile(string path)
    {
      var ret = _file.Entries.FirstOrDefault(x => x.FullName == path);
      if (ret is null)
        throw new Exception($"File '{path}' not found");
      return ret;
    }

    public override IEnumerable<FilePath> GetFiles(FilePath directory) => _file.Entries.Select(x => FilePath.Parse(x.FullName, PathSeparator)).Where(x => x.Nodes.Length == directory.Nodes.Length + 1);
    public override IEnumerable<FilePath> GetDirectories(FilePath directory) => _file.Entries
      .Select(x => FilePath.Parse(x.FullName, PathSeparator))
      .Where(x => x.Nodes.Length == directory.Nodes.Length + 2)
      .Select(x => x.MoveUp())
      .Distinct();
  }
}
