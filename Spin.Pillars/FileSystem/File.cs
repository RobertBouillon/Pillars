using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using io = System.IO;

namespace Spin.Pillars.FileSystem
{
  public abstract class File : ILeaf
  {
    public Provider Provider { get; protected set; }
    public Path Path { get; protected set; }

    public virtual Directory Parent => Path.Count == 0 ? null : Provider.GetDirectory(Path.MoveUp());
    public virtual string Name => Path.Leaf;
    public virtual string Extension => Name.Contains(".") ? Name.Substring(Name.LastIndexOf('.') + 1) : null;
    public virtual string NameLessExtension => Name.Contains(".") ? Name.Substring(0, Name.LastIndexOf('.')) : Name;
    public string FullName => Provider.GetFullPath(Path);

    public File() { }
    public File(Provider provider, Path path)
    {
      #region Validation
      if (provider is null)
        throw new ArgumentNullException(nameof(provider));
      #endregion
      Provider = provider;
      Path = path;
    }

    public abstract io.Stream OpenRead();
    public abstract io.Stream OpenWrite();
    public abstract void Delete();
    public abstract bool Exists();

    public abstract DateTime GetDate(DateStamp stamp, DateTimeKind kind = DateTimeKind.Utc);
    public abstract void SetDate(DateStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc);

    public virtual void CopyTo(Directory directory) => CopyTo(directory.GetFile(Name));
    public virtual void CopyTo(File file, bool overwrite = false)
    {
      if (overwrite && file.Exists())
        file.Delete();

      using var source = OpenRead();
      using var dest = file.OpenWrite();
        source.CopyTo(dest);
    }

    public virtual string ReadAllText(Encoding encoding = null) => encoding is null ? new io.StreamReader(OpenRead()).DisposeAfter(x => x.ReadToEnd()) : new io.StreamReader(OpenRead(), encoding).DisposeAfter(x => x.ReadToEnd());
    public virtual string[] ReadAllLines(Encoding encoding = null) => ReadAllText(encoding).Split('\n');
    public virtual void Write(string text, bool overwrite = true, Encoding encoding = null)
    {
      if (overwrite && Exists())
        Delete();
      (encoding is null ? new io.StreamWriter(OpenWrite()) : new io.StreamWriter(OpenWrite(), encoding)).DisposeAfter(x => x.Write(text));
    }

    public virtual void Write(IEnumerable<string> lines, bool overwrite = true, Encoding encoding = null, string lineDelimiter = null)
    {
      if (overwrite && Exists())
        Delete();
      (encoding is null ? new io.StreamWriter(OpenWrite()) : new io.StreamWriter(OpenWrite(), encoding)).DisposeAfter(x => x.Write(String.Join(lineDelimiter ?? Environment.NewLine, lines)));
    }

    public virtual Task DeleteAsync() { Delete(); return Task.CompletedTask; }
    public virtual Task<bool> ExistsAsync() => Task.FromResult(Exists());
    public virtual Task<io.Stream> OpenReadAsync() => Task.FromResult(OpenRead());
    public virtual Task<io.Stream> OpenWriteAsync() => Task.FromResult(OpenWrite());
    public virtual async Task CopyToAsync(Directory directory) => await CopyToAsync(directory.GetFile(Name));
    public virtual async Task CopyToAsync(File file)
    {
      using var source = await OpenReadAsync();
      using var dest = await file.OpenWriteAsync();
      await source.CopyToAsync(dest);
    }

    public virtual Task<string> ReadAllTextAsync(Encoding encoding = null) => Task.FromResult(ReadAllText());
    public virtual Task<string[]> ReadAllLinesAsync(Encoding encoding = null) => Task.FromResult(ReadAllLines());
    public virtual Task WriteAsync(string text, bool overwrite = true, Encoding encoding = null) { Write(text, overwrite, encoding); return Task.CompletedTask; }

    public override string ToString() => FullName;

    IBranch ILeaf.Parent => Parent;
    Path ILeaf.Path => Path;
  }
}
