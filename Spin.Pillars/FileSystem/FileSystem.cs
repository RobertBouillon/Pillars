using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Text;
using io = System.IO;

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
    public virtual Directory Root => GetDirectory(Path.Empty);
    public virtual bool IsReadOnly => false;

    public FileSystem() { }
    protected FileSystem(string name)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      #endregion
      Name = name;
    }

    public abstract Directory GetDirectory(Path path);
    public abstract File GetFile(Path path);
    public virtual bool IsPathRooted(string path) => (Name != null && path.StartsWith(Name)) || path.StartsWith(PathSeparator.ToString());
    public virtual Path ParsePath(string path) => Path.Parse(path, PathSeparator);
    public virtual Path ParsePath(string path, Path context) => (IsPathRooted(path)) ? ParsePath(path) : context.Append(ParsePath(path));
    public abstract IEnumerable<TimeStamp> SupportedDateStamps { get; }

    public abstract bool FileExists(Path path);
    public abstract bool DirectoryExists(Path path);
    public abstract void DeleteFile(Path path);
    public abstract void DeleteDirectory(Path path);
    public abstract void CreateFile(Path path);
    public abstract void CreateDirectory(Path path);

    public virtual bool FileExists(string path) => FileExists(Path.Parse(path, PathSeparator));
    public virtual bool DirectoryExists(string path) => DirectoryExists(Path.Parse(path, PathSeparator));
    public virtual void DeleteFile(string path) => DeleteFile(Path.Parse(path, PathSeparator));
    public virtual void DeleteDirectory(string path, bool recurse = false) => DeleteDirectory(Path.Parse(path, PathSeparator));
    public virtual void CreateFile(string path) => CreateFile(Path.Parse(path, PathSeparator));
    public virtual void CreateDirectory(string path) => CreateDirectory(Path.Parse(path, PathSeparator));

    public abstract IEnumerable<Path> GetFiles(Path directory);
    public abstract IEnumerable<Path> GetDirectories(Path directory);

    public virtual string GetPathedName(Path path) => Name + PathSeparator + path.ToString(PathSeparator);
    public virtual Path ParseAbsolutePath(string path)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(nameof(path));
      #endregion
      if (path.StartsWith(Name))
        return Path.Parse(path.Substring(path.Length), PathSeparator);
      if (path.StartsWith(PathSeparator.ToString()))
        return Path.Parse(path.Substring(1), PathSeparator);
      return Path.Parse(path, PathSeparator);
    }

    public virtual Path ParsePath(string path, out bool isRooted)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(nameof(path));
      #endregion

      if (path.StartsWith(Name))
      {
        isRooted = true;
        return Path.Parse(path.Substring(path.Length), PathSeparator);
      }
      else if (path.StartsWith(PathSeparator.ToString()))
      {
        isRooted = true;
        return Path.Parse(path.Substring(1), PathSeparator);
      }
      else
      {
        isRooted = false;
        return Path.Parse(path, PathSeparator);
      }
    }

    public virtual bool TryParsePath(string path, out Path pathOut, out bool isRooted)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(nameof(path));
      #endregion

      if (path.StartsWith(Name))
      {
        isRooted = true;
        pathOut = Path.Parse(path.Substring(path.Length), PathSeparator);
      }
      else if (path.StartsWith(PathSeparator.ToString()))
      {
        isRooted = true;
        pathOut = Path.Parse(path.Substring(1), PathSeparator);
      }
      else
      {
        isRooted = false;
        pathOut = Path.Parse(path, PathSeparator);
      }
      return true;
    }
  }
}
