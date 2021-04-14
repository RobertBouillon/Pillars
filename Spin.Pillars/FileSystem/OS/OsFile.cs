using System;
using io = System.IO;
using System.Text;
using Spin.Pillars.Hierarchy;
using System.Reflection;

namespace Spin.Pillars.FileSystem.OS
{
  public class OsFile : File
  {
    public static OsFile CurrentExecuting => new OsFile(Assembly.GetExecutingAssembly().Location);

    public OsProvider OsProvider => Provider as OsProvider;

    private io.FileInfo FileInfo => new io.FileInfo(FullName);

    public override bool IsReadOnly
    {
      get => FileInfo.IsReadOnly;
      set => FileInfo.IsReadOnly = value;
    }

    public override FileSize Size => (FileSize)FileInfo.Length;
    public override OsDirectory Directory => new OsDirectory(OsProvider, Path.MoveUp());
    public override string NameLessExtension => io.Path.GetFileNameWithoutExtension(Name);
    public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind) =>
      stamp == TimeStamp.LastAccess ? kind == DateTimeKind.Utc ? FileInfo.LastAccessTimeUtc : FileInfo.LastAccessTime :
      stamp == TimeStamp.LastWrite ? kind == DateTimeKind.Utc ? FileInfo.LastWriteTimeUtc : FileInfo.LastWriteTime :
      throw new NotImplementedException(stamp.ToString());

    public OsFile(io.FileInfo file) : this(file.FullName) { }
    public OsFile(string path)
    {
      var root = System.IO.Path.GetPathRoot(path);
      if (!OsProvider.Mounts.TryGetValue(root, out var provider))
        throw new ArgumentException($"Unable to find an instance of the drive '{root}'");

      Provider = provider;
      Path = new Path(path.Substring(root.Length), Provider.PathSeparator);
    }

    public OsFile(OsProvider provider, Path path) : base(provider, path) { }

    public override io.Stream OpenRead() => io.File.OpenRead(FullName);
    public override io.Stream OpenWrite() => io.File.OpenWrite(FullName);
    public override void Delete() => io.File.Delete(FullName);
    public override bool Exists() => io.File.Exists(FullName);
    public override string ReadAllText(Encoding encoding = null) => (encoding is null) ? io.File.ReadAllText(FullName) : io.File.ReadAllText(FullName, encoding);
    public override string[] ReadAllLines(Encoding encoding = null) => (encoding is null) ? io.File.ReadAllLines(FullName) : io.File.ReadAllLines(FullName, encoding);

    public override void MoveTo(File file, bool overwrite = false) => io.File.Move(FullName, file.FullName, overwrite);

    public override void Write(string text, bool overwrite = true, Encoding encoding = null)
    {
      if (overwrite)
        Delete();

      if (encoding is null)
        io.File.WriteAllText(OsProvider.GetFullPath(Path), text);
      else
        io.File.WriteAllText(OsProvider.GetFullPath(Path), text, encoding);
    }

    public override void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc)
    {
      if (stamp == TimeStamp.Created)
        if (kind == DateTimeKind.Utc)
          FileInfo.CreationTimeUtc = date;
        else
          FileInfo.CreationTime = date;
      else if (stamp == TimeStamp.LastWrite)
        if (kind == DateTimeKind.Utc)
          FileInfo.LastWriteTimeUtc = date;
        else
          FileInfo.LastWriteTime = date;
      else if (stamp == TimeStamp.LastAccess)
        if (kind == DateTimeKind.Utc)
          FileInfo.CreationTimeUtc = date;
        else
          FileInfo.CreationTime = date;
      else
        throw new NotImplementedException(stamp.ToString());
    }
  }
}
