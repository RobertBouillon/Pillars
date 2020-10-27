using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  public abstract class Directory : IBranch
  {
    public Provider Provider { get; protected set; }
    public Path Path { get; protected set; }

    public Directory() { }
    public Directory(Provider provider, Path path)
    {
      #region Validation
      if (provider is null)
        throw new ArgumentNullException(nameof(provider));
      #endregion
      Provider = provider;
      Path = path;
    }

    public virtual Directory Parent => Path.Count == 0 ? null : Provider.GetDirectory(Path.MoveUp());
    public virtual string Name => Path.Leaf;
    public virtual string FullName => Provider.GetFullPath(Path);

    public abstract IEnumerable<File> GetFiles();
    public abstract IEnumerable<Directory> GetDirectories();

    public virtual IEnumerable<File> FindFiles(Func<File, bool> predicate = null, bool recursive = false) => (recursive ? this.Traverse(x => x.GetDirectories()).SelectMany(x => x.GetFiles()) : GetFiles()).Where(predicate ?? (x => true));
    public virtual IEnumerable<Directory> FindDirectories(Func<Directory, bool> predicate = null, bool recursive = false) => (recursive ? this.Traverse(x => x.Parent).SelectMany(x => x.GetDirectories()) : GetDirectories()).Where(predicate ?? (x => true));

    public virtual Task<IEnumerable<File>> GetFilesAsync() => Task.FromResult(GetFiles());
    public virtual Task<IEnumerable<Directory>> GetDirectoriesAsync() => Task.FromResult(GetDirectories());

    public virtual Directory GetDirectory(string name) => Provider.GetDirectory(Provider.ParsePath(name, Path));
    public virtual File GetFile(string name) => Provider.GetFile(Provider.ParsePath(name, Path));

    public abstract bool Exists();
    public abstract void Create();

    public virtual bool ContainsFile(string name) => GetFile(name).Exists();
    public virtual bool ContainsDirectory(string name) => GetFile(name).Exists();

    public override string ToString() => FullName;

    IEnumerable<ILeaf> IBranch.Children => GetFiles().Cast<ILeaf>().Concat(GetDirectories());
    IEnumerable<IBranch> IBranch.Branches => GetDirectories().Cast<IBranch>();
    IBranch ILeaf.Parent => Parent;
    Path ILeaf.Path => Path;
  }
}
