using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  public abstract class FileSystem
  {
    public static void Copy(File source, File destination, bool overwrite = false)
    {
      #region Validation
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (destination is null)
        throw new ArgumentNullException(nameof(destination));
      #endregion

      if (overwrite && destination.Exists())
        destination.Delete();

      using (var reader = source.OpenRead())
      using (var writer = destination.OpenWrite())
        reader.CopyTo(writer);
    }

    public static void Move(File source, File destination, bool overwrite = false)
    {
      #region Validation
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (destination is null)
        throw new ArgumentNullException(nameof(destination));
      #endregion

      if (overwrite && destination.Exists())
        destination.Delete();

      Copy(source, destination);
      source.Delete();
    }

    public string Name { get; }
    public abstract char PathSeparator { get; }
    public virtual Directory Root { get; }
    public virtual bool IsReadOnly => false;

    public FileSystem() => Root = GetDirectory(Path.Root);
    protected FileSystem(string name)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      #endregion
      Name = name;
    }

    public virtual Directory GetDirectory(string path) => GetDirectory(new Path(ParsePath(path), isTerminated: true));
    public abstract Directory GetDirectory(Path path);
    public virtual File GetFile(string path) => GetFile(ParsePath(path));
    public abstract File GetFile(Path path);
    public virtual Path ParsePath(string path) => Path.Parse(path, PathSeparator);
    public virtual Path ParsePath(string path, Path context) => context.Append(ParsePath(path));
    public abstract IEnumerable<TimeStamp> SupportedDateStamps { get; }

    public abstract bool FileExists(Path path);
    public abstract bool DirectoryExists(Path path);
    public abstract void DeleteFile(Path path);
    public abstract void DeleteDirectory(Path path);
    public abstract void CreateFile(Path path);
    public abstract void CreateDirectory(Path path);
    public virtual void RenameFile(Path path, string name)
    {
      var source = GetFile(path);
      source.MoveTo(source.ParentDirectory.GetFile(name));
    }

    public virtual bool FileExists(string path) => FileExists(Path.Parse(path, PathSeparator));
    public virtual bool DirectoryExists(string path) => DirectoryExists(Path.Parse(path, PathSeparator));
    public virtual void DeleteFile(string path) => DeleteFile(Path.Parse(path, PathSeparator));
    public virtual void DeleteDirectory(string path, bool recurse = false) => DeleteDirectory(Path.Parse(path, PathSeparator));
    public virtual void CreateFile(string path) => CreateFile(Path.Parse(path, PathSeparator));
    public virtual void CreateDirectory(string path) => CreateDirectory(Path.Parse(path, PathSeparator));

    public virtual Task<IEnumerable<Path>> GetFilesAsync(Path directory) => Task.FromResult(GetFiles(directory));
    public virtual Task<IEnumerable<Path>> GetDirectoriesAsync(Path directory) => Task.FromResult(GetDirectories(directory));
    public abstract IEnumerable<Path> GetFiles(Path directory);
    public abstract IEnumerable<Path> GetDirectories(Path directory);

    public virtual string GetPathedName(Path path) => path.ToString(PathSeparator);

    public virtual IEntity this[string path] => this[ParsePath(path)];
    public virtual IEntity this[Path path] =>
      path.IsTerminated == true ? GetFile(path) :
      path.IsTerminated == false ? GetDirectory(path) :
      (IEntity)GetFile(path) ?? GetDirectory(path);
  }
}
