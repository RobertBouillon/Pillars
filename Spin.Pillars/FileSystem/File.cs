using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using io = System.IO;

namespace Spin.Pillars.FileSystem
{
  public abstract class File : Disposable, IEntity
  {
    public virtual FileSystem FileSystem { get; }
    public Path Path { get; }

    public virtual Directory ParentDirectory => Path.Count == 0 ? null : FileSystem.GetDirectory(Path.MoveUp());
    public virtual string Name => Path.Leaf;
    public virtual string Extension => Name.Contains(".") ? Name.Substring(Name.LastIndexOf('.') + 1) : null;
    public virtual string NameLessExtension => Name.Contains(".") ? Name.Substring(0, Name.LastIndexOf('.')) : Name;
    public string PathedName => FileSystem.GetPathedName(Path);

    public File(FileSystem fileSystem, Path path)
    {
      #region Validation
      if (fileSystem is null)
        throw new ArgumentNullException(nameof(fileSystem));
      #endregion
      FileSystem = fileSystem;
      Path = path;
    }

    public abstract io.Stream OpenRead();
    public abstract io.Stream OpenWrite();
    public abstract bool IsReadOnly { get; set; }
    public abstract FileSize Size { get; }

    public virtual bool Cache() => false;
    public virtual void ClearCache() { }
    public virtual bool IsCached => false;
    public virtual bool IsLocked => false;

    public abstract DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind = DateTimeKind.Utc);
    public abstract void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc);

    public virtual void Create() => FileSystem.CreateFile(Path);
    public virtual bool Exists() => FileSystem.FileExists(Path);
    public virtual void Delete() => FileSystem.DeleteFile(Path);
    public virtual void Rename(string name) => FileSystem.RenameFile(Path, name);

    public void CopyTo(Directory directory, bool overwrite = false) => CopyTo(directory.GetFile(Name), overwrite);
    public virtual void CopyTo(File file, bool overwrite = false) => FileSystem.Copy(this, file, overwrite);

    public void MoveTo(Directory directory, bool overwrite = false) => CopyTo(directory.GetFile(Name), overwrite);
    public virtual void MoveTo(File file, bool overwrite = false)
    {
      #region Validation
      if (file is null)
        throw new ArgumentNullException(nameof(file));
      #endregion
      CopyTo(file, overwrite);
      Delete();
    }

    public virtual byte[] ReadAllBytes() => new io.MemoryStream().DisposeAfter(x => { OpenRead().DisposeAfter(y => y.CopyTo(x)); return x.ToArray(); });
    public virtual string ReadAllText(Encoding encoding = null) => encoding is null ? new io.StreamReader(OpenRead()).DisposeAfter(x => x.ReadToEnd()) : new io.StreamReader(OpenRead(), encoding).DisposeAfter(x => x.ReadToEnd());
    public virtual string[] ReadAllLines(Encoding encoding = null) => ReadAllText(encoding).Split('\n');
    public virtual void Write(string text, bool overwrite = true, Encoding encoding = null)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(text))
        throw new ArgumentNullException(nameof(text));
      #endregion

      if (overwrite && Exists())
        Delete();
      (encoding is null ? new io.StreamWriter(OpenWrite()) : new io.StreamWriter(OpenWrite(), encoding)).DisposeAfter(x => x.Write(text));
    }

    public virtual void Write(IEnumerable<string> lines, bool overwrite = true, Encoding encoding = null, string lineDelimiter = null)
    {
      #region Validation
      if (lines is null)
        throw new ArgumentNullException(nameof(lines));
      #endregion
      if (overwrite && Exists())
        Delete();
      (encoding is null ? new io.StreamWriter(OpenWrite()) : new io.StreamWriter(OpenWrite(), encoding)).DisposeAfter(x => x.Write(string.Join(lineDelimiter ?? System.Environment.NewLine, lines)));
    }

    public bool WaitExists(TimeSpan? pollInterval = null, TimeSpan? timeout = null)
    {
      pollInterval ??= TimeSpan.FromMilliseconds(100);
      timeout ??= TimeSpan.Zero;

      var timer = new Stopwatch();
      timer.Start();

      while (!Exists())
      {
        Thread.Sleep(pollInterval.Value);
        if (timeout == TimeSpan.Zero || timeout < timer.Elapsed)
          return false;
      }

      return true;
    }

    #region Async

    public virtual Task DeleteAsync() { Delete(); return Task.CompletedTask; }
    public virtual Task<bool> ExistsAsync() => Task.FromResult(Exists());
    public virtual Task<io.Stream> OpenReadAsync() => Task.FromResult(OpenRead());
    public virtual Task<io.Stream> OpenWriteAsync() => Task.FromResult(OpenWrite());
    public virtual async Task CopyToAsync(Directory directory) => await CopyToAsync(directory.GetFile(Name));
    public virtual async Task CopyToAsync(File file)
    {
      #region Validation
      if (file is null)
        throw new ArgumentNullException(nameof(file));
      #endregion
      using var source = await OpenReadAsync();
      using var dest = await file.OpenWriteAsync();
      await source.CopyToAsync(dest);
    }

    public virtual Task<byte[]> ReadAllBytesAsync(Encoding encoding = null) => Task.FromResult(ReadAllBytes());
    public virtual Task<string> ReadAllTextAsync(Encoding encoding = null) => Task.FromResult(ReadAllText());
    public virtual Task<string[]> ReadAllLinesAsync(Encoding encoding = null) => Task.FromResult(ReadAllLines());
    public virtual Task WriteAsync(string text, bool overwrite = true, Encoding encoding = null) { Write(text, overwrite, encoding); return Task.CompletedTask; }
    #endregion

    public override string ToString() => PathedName;

    IBranch ILeaf.Parent => ParentDirectory;
    Path ILeaf.Path => Path;
  }
}
