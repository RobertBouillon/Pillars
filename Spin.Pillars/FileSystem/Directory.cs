using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  public abstract class Directory : IBranch, IEntity
  {
    public virtual FileSystem FileSystem { get; }
    public Path Path { get; }

    public Directory() { }
    public Directory(FileSystem fileSystem, Path path)
    {
      #region Validation
      if (fileSystem is null)
        throw new ArgumentNullException(nameof(fileSystem));
      #endregion
      FileSystem = fileSystem;
      Path = path.Simplify();
    }

    public virtual Directory ParentDirectory => Path.Count == 0 ? null : FileSystem.GetDirectory(Path.MoveUp());
    public virtual string Name => Path.Leaf ?? FileSystem.PathSeparator.ToString();
    public virtual string PathedName => FileSystem.GetPathedName(Path);

    public virtual IEnumerable<File> GetFiles() => FileSystem.GetFiles(Path).Select(x => FileSystem.GetFile(x));
    public virtual IEnumerable<Directory> GetDirectories() => FileSystem.GetDirectories(Path).Select(x => FileSystem.GetDirectory(x));

    public virtual IEnumerable<File> FindFiles(Func<File, bool> predicate = null, bool recursive = false) => (recursive ? this.Traverse(x => x.GetDirectories()).SelectMany(x => x.GetFiles()) : GetFiles()).Where(predicate ?? (x => true));
    public virtual IEnumerable<Directory> FindDirectories(Func<Directory, bool> predicate = null, bool recursive = false) => (recursive ? this.Traverse(x => x.ParentDirectory).SelectMany(x => x.GetDirectories()) : GetDirectories()).Where(predicate ?? (x => true));

    public virtual async Task<IEnumerable<File>> GetFilesAsync() => (await FileSystem.GetFilesAsync(Path)).Select(x => FileSystem.GetFile(x));
    public virtual async Task<IEnumerable<Directory>> GetDirectoriesAsync() => (await FileSystem.GetDirectoriesAsync(Path)).Select(x => FileSystem.GetDirectory(x));

    public virtual Directory GetDirectory(Path path) => FileSystem.GetDirectory(Path.Append(path));
    public virtual Directory GetDirectory(string name) => FileSystem.GetDirectory(FileSystem.ParsePath(name, Path));
    public virtual File GetFile(Path path) => FileSystem.GetFile(Path.Append(path));
    public virtual File GetFile(string name) => FileSystem.GetFile(FileSystem.ParsePath(name, Path));

    //Optional caching. Not implemented / supported by default.
    public virtual bool Cache() => false;
    public virtual void ClearCache() { }
    public virtual bool IsCached => false;

    public virtual void MoveTo(Directory destination, bool overwrite = true, bool recurse = false)
    {
      foreach (var file in GetFiles())
        file.MoveTo(destination, overwrite);

      if (recurse)
        foreach (var directory in GetDirectories())
          directory.CopyTo(destination.Create(directory.Name), overwrite, true);
    }

    public virtual void CopyTo(Directory destination, bool overwrite = true, bool recurse = false)
    {
      foreach (var file in GetFiles())
        file.CopyTo(destination, overwrite);

      if (recurse)
        foreach (var directory in GetDirectories())
          directory.CopyTo(destination.Create(directory.Name), overwrite, true);
    }

    public virtual Task<bool> ExistsAsync() => FileSystem.DirectoryExistsAsync(Path);
    public virtual Task CreateAsync() => FileSystem.CreateDirectoryAsync(Path);
    public virtual Task DeleteAsync() => FileSystem.DeleteDirectoryAsync(Path);

    public virtual bool Exists() => FileSystem.DirectoryExists(Path);
    public virtual void Create() => FileSystem.CreateDirectory(Path);
    public virtual void Delete() => FileSystem.DeleteDirectory(Path);
    public virtual void Purge()
    {
      foreach (var file in GetFiles())
        file.Delete();
      foreach (var directory in GetDirectories())
        directory.Purge();
    }

    public virtual Directory Create(string subdirectory)
    {
      var sub = GetDirectory(subdirectory);
      if (sub.Exists())
        throw new Exception($"{subdirectory} already exists");

      sub.Create();
      return sub;
    }

    public virtual Task<bool> ContainsFileAsync(string name) => FileSystem.FileExistsAsync(Path.Append(name));
    public virtual bool ContainsFile(string name) => FileSystem.FileExists(Path.Append(name));
    public virtual bool ContainsDirectory(string name) => FileSystem.DirectoryExists(Path.Append(name));

    public override string ToString() => PathedName;

    IEnumerable<ILeaf> IBranch.Children => GetFiles().Cast<ILeaf>().Concat(GetDirectories());
    IEnumerable<IBranch> IBranch.Branches => GetDirectories().Cast<IBranch>();
    IBranch ILeaf.Parent => ParentDirectory;
    Path ILeaf.Path => Path;
  }
}
