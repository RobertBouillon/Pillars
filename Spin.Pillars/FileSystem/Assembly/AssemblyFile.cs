using System;
using io = System.IO;
using System.Text;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.Assembly
{
  public class AssemblyFile : File
  {
    public override AssemblyFileSystem FileSystem => base.FileSystem as AssemblyFileSystem;

    public override Directory ParentDirectory => new AssemblyDirectory(FileSystem, Path.MoveUp());

    public override bool IsReadOnly
    {
      get => true;
      set => throw new NotSupportedException();
    }

    public override FileSize Size => FileSystem.Assembly.GetManifestResourceStream(PathedName).Length;

    public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind) => throw new NotSupportedException(stamp.ToString());

    public AssemblyFile(AssemblyFileSystem fileSystem, string path) : this(fileSystem, fileSystem.ParsePath(path)) { }
    public AssemblyFile(AssemblyFileSystem fileSystem, FilePath path) : base(fileSystem, path) { }

    public override io.Stream OpenRead() => FileSystem.Assembly.GetManifestResourceStream(PathedName);
    public override io.Stream OpenWrite() => throw new NotSupportedException();

    public override void Write(string text, bool overwrite = true, Encoding encoding = null) => throw new NotSupportedException();
    public override void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc) => throw new NotSupportedException();
  }
}
