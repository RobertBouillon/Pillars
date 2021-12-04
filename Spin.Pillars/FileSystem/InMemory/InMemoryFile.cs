using Spin.Pillars.Hierarchy;
using System;
using System.Text;
using io = System.IO;

namespace Spin.Pillars.FileSystem.InMemory
{
  public class InMemoryFile : File
  {
    public override InMemoryFileSystem FileSystem => base.FileSystem as InMemoryFileSystem;

    private MemoryFile _memoryFile;
    private MemoryFile MemoryFile => _memoryFile ??= FileSystem.FindFile(Path);

    public override bool IsReadOnly
    {
      get => MemoryFile.IsReadOnly;
      set => MemoryFile.IsReadOnly = value;
    }

    public override FileSize Size => MemoryFile is null ? throw new Exception("File not found") : MemoryFile.Size;
    public override InMemoryDirectory ParentDirectory => new InMemoryDirectory(FileSystem, Path.MoveUp());
    public override string NameLessExtension => io.Path.GetFileNameWithoutExtension(Name);

    public override bool IsCached => _memoryFile is not null;

    public override DateTime GetTimeStamp(TimeStamp stamp, DateTimeKind kind) =>
      stamp == TimeStamp.LastAccess ? MemoryFile.LastAccessTime :
      stamp == TimeStamp.LastWrite ? MemoryFile.LastWriteTime :
      throw new NotImplementedException(stamp.ToString());

    internal InMemoryFile(InMemoryFileSystem fileSystem, MemoryFile file) : base(fileSystem, file.Path) { }
    public InMemoryFile(InMemoryFileSystem fileSystem, Path path) : base(fileSystem, path) { }

    public override io.Stream OpenRead()
    {
      if (MemoryFile is null)
        throw new Exception($"File not found: " + PathedName);
      return MemoryFile.Stream;
    }

    public override io.Stream OpenWrite()
    {
      if (MemoryFile is null)
        FileSystem.CreateFile(Path);
      return MemoryFile.Stream;
    }
    public override void Delete() => MemoryFile.Delete();
    public override bool Exists() => FileSystem.FindFile(Path) is not null;

    public override void CopyTo(File file, bool overwrite = false)
    {
      #region Validation
      if (file is null)
        throw new ArgumentNullException(nameof(file));
      #endregion
      if (file is InMemoryFile mf)
      {
        var md = mf.ParentDirectory.MemoryDirectory;
        if (md.Files.TryGetValue(file.Name, out var existing))
          if (overwrite)
            throw new Exception($"{file.Name} already exists in {mf.ParentDirectory}");
          else
            existing.Delete();

        md.Files[file.Name] = _memoryFile.Clone(md);
      }
      else
        base.CopyTo(file, overwrite);
    }

    public override void MoveTo(File file, bool overwrite = false)
    {
      #region Validation
      if (file is null)
        throw new ArgumentNullException(nameof(file));
      #endregion
      if (file is InMemoryFile mf)
        MemoryFile.Directory = mf.MemoryFile.Directory;
      else
        base.MoveTo(file, overwrite);
    }

    public override void SetDate(TimeStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc)
    {
      if (stamp == TimeStamp.Created)
        MemoryFile.CreationTime = date;
      else if (stamp == TimeStamp.LastWrite)
        MemoryFile.LastWriteTime = date;
      else if (stamp == TimeStamp.LastAccess)
        MemoryFile.LastAccessTime = date;
      else
        throw new NotImplementedException(stamp.ToString());
    }

    public override bool Cache() => (_memoryFile = FileSystem.FindFile(Path)) is not null;
    public override void ClearCache() => _memoryFile = null;
    protected override void DisposeManaged() => _memoryFile.Dispose();
  }
}
