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

    public FileSystem() { }
    protected FileSystem(string name)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      #endregion
      Name = name;
      Root = GetDirectory(new FilePath(Path.Empty, true, false));
    }

    public virtual Directory GetDirectory(string path)
    {
      var filepath = ParsePath(path);
      filepath.IsLeafDirectory = true;
      return GetDirectory(path);
    }

    public abstract Directory GetDirectory(FilePath path);
    public virtual File GetFile(string path) => GetFile(ParsePath(path));
    public abstract File GetFile(FilePath path);
    public virtual FilePath ParsePath(string path) => FilePath.Parse(path, PathSeparator);
    public virtual FilePath ParsePath(string path, FilePath context) => FilePath.IsPathRooted(path, PathSeparator) ? ParsePath(path) : context.Append(ParsePath(path));
    public abstract IEnumerable<TimeStamp> SupportedDateStamps { get; }

    public abstract bool FileExists(FilePath path);
    public abstract bool DirectoryExists(FilePath path);
    public abstract void DeleteFile(FilePath path);
    public abstract void DeleteDirectory(FilePath path);
    public abstract void CreateFile(FilePath path);
    public abstract void CreateDirectory(FilePath path);
    public virtual void RenameFile(FilePath path, string name)
    {
      var source = GetFile(path);
      source.MoveTo(source.ParentDirectory.GetFile(name));
    }

    public virtual bool FileExists(string path) => FileExists(FilePath.Parse(path, PathSeparator));
    public virtual bool DirectoryExists(string path) => DirectoryExists(FilePath.Parse(path, PathSeparator));
    public virtual void DeleteFile(string path) => DeleteFile(FilePath.Parse(path, PathSeparator));
    public virtual void DeleteDirectory(string path, bool recurse = false) => DeleteDirectory(FilePath.Parse(path, PathSeparator));
    public virtual void CreateFile(string path) => CreateFile(FilePath.Parse(path, PathSeparator));
    public virtual void CreateDirectory(string path) => CreateDirectory(FilePath.Parse(path, PathSeparator));

    public virtual Task<IEnumerable<FilePath>> GetFilesAsync(FilePath directory) => Task.FromResult(GetFiles(directory));
    public virtual Task<IEnumerable<FilePath>> GetDirectoriesAsync(FilePath directory) => Task.FromResult(GetDirectories(directory));
    public abstract IEnumerable<FilePath> GetFiles(FilePath directory);
    public abstract IEnumerable<FilePath> GetDirectories(FilePath directory);

    public virtual string GetPathedName(FilePath path) => path.ToString(PathSeparator);
    public virtual Path ParseAbsolutePath(string path)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(nameof(path));
      #endregion
      if (path.StartsWith(Name))
        return FilePath.Parse(path.Substring(path.Length), PathSeparator);
      if (path.StartsWith(PathSeparator.ToString()))
        return FilePath.Parse(path.Substring(1), PathSeparator);
      return FilePath.Parse(path, PathSeparator);
    }

    public virtual IEntity this[string path] => this[ParsePath(path)];
    public virtual IEntity this[FilePath path] =>
      path.IsLeafDirectory == true ? GetFile(path) :
      path.IsLeafDirectory == false ? GetDirectory(path) :
      (IEntity)GetFile(path) ?? GetDirectory(path);

    public virtual IEntity this[Path path] => this[new FilePath(path)];
  }
}
