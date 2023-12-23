using System;
using io = System.IO;
using System.Text;
using Spin.Pillars.Hierarchy;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.OS;

public partial class OsFile : File
{
  public static OsFile CurrentExecuting => Parse(System.Reflection.Assembly.GetExecutingAssembly().Location);
  public static OsFile Create(io.FileInfo file) => Parse(file.FullName);
  public static OsFile Parse(string path) => new OsFile(new OsFileSystem(), WindowsFilePath.Parse(path));

  public override OsFileSystem FileSystem => base.FileSystem as OsFileSystem;

  private io.FileInfo _fileInfo;
  private io.FileInfo FileInfo => _fileInfo ?? new io.FileInfo(PathedName);

  public override bool IsReadOnly
  {
    get => FileInfo.IsReadOnly;
    set => FileInfo.IsReadOnly = value;
  }

  public override FileSize Size => FileInfo.Length;
  public override OsDirectory ParentDirectory => new OsDirectory(FileSystem, Path.MoveUp());
  public override string NameLessExtension => io.Path.GetFileNameWithoutExtension(Name);
  public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind) =>
    stamp == TimeStamp.LastAccess ? kind == DateTimeKind.Utc ? FileInfo.LastAccessTimeUtc : FileInfo.LastAccessTime :
    stamp == TimeStamp.LastWrite ? kind == DateTimeKind.Utc ? FileInfo.LastWriteTimeUtc : FileInfo.LastWriteTime :
    throw new NotImplementedException(stamp.ToString());

  public OsFile(OsFileSystem fileSystem, Path path) : base(fileSystem, path) { }

  public override io.Stream OpenRead() => io.File.OpenRead(PathedName);
  public override io.Stream OpenWrite() => io.File.OpenWrite(PathedName);

  public override string ReadAllText(Encoding encoding = null) => (encoding is null) ? io.File.ReadAllText(PathedName) : io.File.ReadAllText(PathedName, encoding);
  public override string[] ReadAllLines(Encoding encoding = null) => (encoding is null) ? io.File.ReadAllLines(PathedName) : io.File.ReadAllLines(PathedName, encoding);

  public override bool Cache() => (_fileInfo = new io.FileInfo(PathedName)) is not null;
  public override void ClearCache() => _fileInfo = null;
  public override bool IsCached => _fileInfo is not null;

  public override void MoveTo(File file, bool overwrite = false)
  {
    #region Validation
    if (file is null)
      throw new ArgumentNullException(nameof(file));
    #endregion
    io.File.Move(PathedName, file.PathedName, overwrite);
  }

  public override void Write(string text, bool overwrite = true, Encoding encoding = null)
  {
    #region Validation
    if (text is null)  //Allow whitespace
      throw new ArgumentNullException(nameof(text));
    #endregion

    if (overwrite && Exists())
      Delete();

    if (encoding is null)
      io.File.WriteAllText(FileSystem.GetPathedName(Path), text);
    else
      io.File.WriteAllText(FileSystem.GetPathedName(Path), text, encoding);
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
        FileInfo.LastAccessTime = date;
      else
        FileInfo.LastAccessTimeUtc = date;
    else
      throw new NotImplementedException(stamp.ToString());
  }

  public Attempt TryUnlock(TimeSpan timeout)
  {
    Stopwatch sw = Stopwatch.StartNew();
    while (IsLocked)
    {
      Thread.Sleep(50);
      if (sw.Elapsed > timeout)
        return Attempt.Failure;
    }
    return Attempt.Successful;
  }

  public async Task<Attempt> TryUnlockAsync(TimeSpan timeout)
  {
    Stopwatch sw = Stopwatch.StartNew();
    while (IsLocked)
    {
      await Task.Delay(50);
      if (sw.Elapsed > timeout)
        return Attempt.Failure;
    }
    return Attempt.Successful;
  }

  public override bool IsLocked
  {
    get
    {
      try
      {
        using (var stream = io.File.OpenRead(PathedName))
          return false;
      }
      catch (io.IOException)
      {
        return true;
      }
    }
  }
}
