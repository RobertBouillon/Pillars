using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spin.Pillars.FileSystem
{
  public abstract class Provider
  {
    public string Name { get; }
    public char PathSeparator { get; protected set; } = '/';
    public Directory Root => GetDirectory(Path.Empty);

    public Provider() { }
    protected Provider(string name)
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
    public virtual Path ParsePath(string path) => new Path(path, PathSeparator);
    public virtual Path ParsePath(string path, Path context) => (IsPathRooted(path)) ? ParsePath(path) : context.Append(ParsePath(path));
    public abstract DateStamp[] SupportedDateStamps { get; }

    public virtual string GetFullPath(Path path) => Name + PathSeparator + path.ToString(PathSeparator);
    public virtual Path ParseAbsolutePath(string path)
    {
      if (path.StartsWith(Name))
        return new Path(path.Substring(path.Length), PathSeparator);
      if (path.StartsWith(PathSeparator.ToString()))
        return new Path(path.Substring(1), PathSeparator);
      return new Path(path, PathSeparator);
    }

    public virtual Path ParsePath(string path, out bool isRooted)
    {
      if (path.StartsWith(Name))
      {
        isRooted = true;
        return new Path(path.Substring(path.Length), PathSeparator);
      }
      else if (path.StartsWith(PathSeparator.ToString()))
      {
        isRooted = true;
        return new Path(path.Substring(1), PathSeparator);
      }
      else
      {
        isRooted = false;
        return new Path(path, PathSeparator);
      }
    }

    public virtual bool TryParsePath(string path, out Path pathOut, out bool isRooted)
    {
      if (path.StartsWith(Name))
      {
        isRooted = true;
        pathOut = new Path(path.Substring(path.Length), PathSeparator);
      }
      else if (path.StartsWith(PathSeparator.ToString()))
      {
        isRooted = true;
        pathOut = new Path(path.Substring(1), PathSeparator);
      }
      else
      {
        isRooted = false;
        pathOut = new Path(path, PathSeparator);
      }
      return true;
    }

  }
}
