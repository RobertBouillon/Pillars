using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public virtual Directory GetDirectory(string path) => GetDirectory(ParsePath(path));
    public abstract Directory GetDirectory(Path path);
    public virtual File GetFile(string path) => GetFile(ParsePath(path));
    public abstract File GetFile(Path path);
    public virtual Path ParsePath(string path) => Path.Parse(path, PathSeparator);
    public virtual Path ParsePath(string path, Path context) => Path.IsPathRooted(path, PathSeparator) ? ParsePath(path) : context.Append(ParsePath(path));
    public abstract IEnumerable<TimeStamp> SupportedDateStamps { get; }

    public virtual Task<bool> FileExistsAsync(string path) => FileExistsAsync(ParsePath(path));
    public virtual Task<bool> DirectoryExistsAsync(string path) => DirectoryExistsAsync(ParsePath(path));
    public virtual Task DeleteFileAsync(string path) => DeleteFileAsync(ParsePath(path));
    public virtual Task DeleteDirectoryAsync(string path, bool recurse = false) { DeleteDirectory(ParsePath(path), recurse); return Task.CompletedTask; }
    public virtual Task CreateFileAsync(string path) { CreateFile(ParsePath(path)); return Task.CompletedTask; }
    public virtual Task CreateDirectoryAsync(string path) { CreateDirectory(ParsePath(path)); return Task.CompletedTask; }

    public virtual Task<bool> FileExistsAsync(Path path) => Task.FromResult(FileExists(path));
    public virtual Task<bool> DirectoryExistsAsync(Path path) => Task.FromResult(DirectoryExists(path));
    public virtual Task DeleteFileAsync(Path path) { DeleteFile(path); return Task.CompletedTask; }
    public virtual Task DeleteDirectoryAsync(Path path) { DeleteDirectory(path); return Task.CompletedTask; }
    public virtual Task CreateFileAsync(Path path) { CreateFile(path); return Task.CompletedTask; }
    public virtual Task CreateDirectoryAsync(Path path) { CreateDirectory(path); return Task.CompletedTask; }

    public abstract bool FileExists(Path path);
    public abstract bool DirectoryExists(Path path);
    public abstract void DeleteFile(Path path);
    public abstract void DeleteDirectory(Path path, bool recurse = false);
    public abstract void CreateFile(Path path);
    public abstract void CreateDirectory(Path path);
    public virtual void RenameFile(Path path, string name)
    {
      var source = GetFile(path);
      source.MoveTo(source.ParentDirectory.GetFile(name));
    }

    public virtual bool FileExists(string path) => FileExists(ParsePath(path));
    public virtual bool DirectoryExists(string path) => DirectoryExists(ParsePath(path));
    public virtual void DeleteFile(string path) => DeleteFile(ParsePath(path));
    public virtual void DeleteDirectory(string path, bool recurse = false) => DeleteDirectory(ParsePath(path));
    public virtual void CreateFile(string path) => CreateFile(ParsePath(path));
    public virtual void CreateDirectory(string path) => CreateDirectory(ParsePath(path));

    public virtual Task<IEnumerable<Path>> GetFilesAsync(Path directory) => Task.FromResult(GetFiles(directory));
    public virtual Task<IEnumerable<Path>> GetDirectoriesAsync(Path directory) => Task.FromResult(GetDirectories(directory));
    public abstract IEnumerable<Path> GetFiles(Path directory);
    public abstract IEnumerable<Path> GetDirectories(Path directory);

    public virtual string GetPathedName(Path path) => path.ToString(PathSeparator);
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

    public virtual IEntity this[string path] => this[ParsePath(path)];
    public virtual IEntity this[Path path] => path.IsBranch ? GetDirectory(path) : GetFile(path);
  }
}
