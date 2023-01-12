using Spin.Pillars.Hierarchy;
using System;
using io = System.IO;
using System.IO.Compression;

namespace Spin.Pillars.FileSystem.Zip;

public class ZipFile : File
{
  public override ZipFileSystem FileSystem => base.FileSystem as ZipFileSystem;

  private ZipArchiveEntry _zipEntry;
  public ZipArchiveEntry ZipEntry => _zipEntry ?? FileSystem.FindFile(Path);

  public override bool IsReadOnly
  {
    get => FileSystem.IsReadOnly;
    set => throw new NotSupportedException();
  }

  public override FileSize Size => _zipEntry is null ? throw new Exception("File not found") : _zipEntry.Length;
  public FileSize CompressedSize => _zipEntry is null ? throw new Exception("File not found") : _zipEntry.CompressedLength;

  public override ZipDirectory ParentDirectory => new ZipDirectory(FileSystem, Path.MoveUp());

  public override bool IsCached => _zipEntry is not null;

  public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind) =>
    stamp == TimeStamp.LastWrite ? ZipEntry.LastWriteTime.ToUniversalTime().DateTime :
    throw new NotImplementedException(stamp.ToString());

  internal ZipFile(ZipFileSystem fileSystem, ZipFile file) : base(fileSystem, file.Path) { }
  public ZipFile(ZipFileSystem fileSystem, Path path) : base(fileSystem, path) { }

  public override io.Stream OpenRead() => IsReadOnly ? ZipEntry.Open() : throw new Exception("Archive is write-only");
  public override io.Stream OpenWrite() => !IsReadOnly ? ZipEntry.Open() : throw new Exception("Archive is read-only");
  public override void Delete() => ZipEntry.Delete();
  public override bool Exists() => FileSystem.FindFile(Path) is not null;

  public override void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc)
  {
    if (stamp == TimeStamp.LastWrite)
      ZipEntry.LastWriteTime = date;
    else
      throw new NotImplementedException(stamp.ToString());
  }

  public override bool Cache() => (_zipEntry = FileSystem.FindFile(Path)) is not null;
  public override void ClearCache() => _zipEntry = null;
}
