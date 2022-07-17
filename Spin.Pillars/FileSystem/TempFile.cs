using System;
using io = System.IO;

namespace Spin.Pillars.FileSystem.OS
{
  public class TempFile : File
  {
    public File File { get; }
    public override bool IsReadOnly { get => File.IsReadOnly; set => File.IsReadOnly = value; }

    public override FileSize Size => File.Size;

    ~TempFile() => Dispose(false);

    public TempFile(File file) : base(file.FileSystem, file.Path) => File = file;
    protected override void DisposeNative() => Delete();

    public override io.Stream OpenRead() => File.OpenRead();
    public override io.Stream OpenWrite() => File.OpenWrite();
    public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind = DateTimeKind.Utc) => File.GetTimeStamp(stamp, kind);
    public override void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc) => SetDate(stamp, date, kind);
  }
}
