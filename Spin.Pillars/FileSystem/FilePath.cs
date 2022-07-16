using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spin.Pillars.FileSystem
{
  public class FilePath : Path, IComparable<FilePath>
  {
    public static bool IsPathRooted(string path, char separator) => path.StartsWith(separator);
    public static bool IsLeaf(string path, char separator) => path.EndsWith(separator);
    public static new FilePath Parse(string path, char separator)
    {
      #region Validation
      if (String.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(nameof(path));
      #endregion

      char? driveLetter = path.Length > 2 && path[1] == ':' ? path[0] : null;
      var isrooted = false;
      var isdir = false;

      if (driveLetter.HasValue)
        path = path.Substring(2);

      if (path[0] == separator)
        isrooted = true;

      if (path.Length > 2 && path[path.Length - 1] == separator)
        isdir = true;

      var p =
        isrooted && isdir ? Path.Parse(path.Substring(1, path.Length - 2), separator) :
        isrooted ? Path.Parse(path.Substring(1, path.Length - 2), separator) :
        isdir ? Path.Parse(path.Substring(1, path.Length - 2), separator) :
        Path.Parse(path, separator);

      return !driveLetter.HasValue ?
        new(p, isrooted, isdir) :
        new OS.WindowsFilePath(p, isrooted, isdir, driveLetter);
    }

    public static new FilePath Parse(string path, string separator) => new(path.Split(new[] { separator }, StringSplitOptions.None));

    public static new IEnumerable<FilePath> Parse(IEnumerable<string> items)
    {
      var intern = new HashSet<string>();

      foreach (var item in items)
      {
        var path = new FilePath(item);
        path.Intern(intern);
        yield return path;
      }
    }

    public bool IsRooted { get; set; }
    public bool IsLeafDirectory { get; set; }

    public FilePath(Path path) => Nodes = path.Nodes;
    public FilePath(params string[] nodes) : base(nodes) { }
    public FilePath(string[] nodes, bool isRooted = false, bool isLeafDirectory = false) : base(nodes) => (IsRooted, IsLeafDirectory) = (isRooted, isLeafDirectory);
    public FilePath(IEnumerable<string> nodes) : base(nodes) { }
    public FilePath(ILeaf node) : base(node) { }
    public FilePath(Path node, bool isRooted = false, bool isLeafDirectory = false) : this(node.Nodes) => (IsRooted, IsLeafDirectory) = (isRooted, isLeafDirectory);

    public override FilePath Append(params string[] paths) => new(base.Append(paths)) { IsRooted = IsRooted, IsLeafDirectory = IsLeafDirectory };
    public override FilePath Append(Path path) => new(base.Append(path)) { IsRooted = IsRooted, IsLeafDirectory = IsLeafDirectory };

    public override FilePath MoveUp(int levels = 1) => new FilePath(base.MoveUp(), IsRooted, true);

    public int CompareTo(FilePath other)
    {
      int ret;

      if ((ret = IsRooted.CompareTo(other.IsRooted)) != 0)
        return ret;

      if ((ret = IsLeafDirectory.CompareTo(other.IsLeafDirectory)) != 0)
        return ret;

      return base.CompareTo(other);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), IsRooted, IsLeafDirectory);

    public override bool Equals(object obj) => Equals(obj as FilePath);
    public bool Equals(FilePath o) =>
      o is not null &&
      IsRooted == o.IsRooted &&
      IsLeafDirectory == o.IsLeafDirectory &&
      base.Equals(o);

    public override string ToString() => ToString('\\');

    public override string ToString(char separator) =>
      IsRooted ? separator + Nodes.Join(separator) :
      IsRooted && IsLeafDirectory ? separator + Nodes.Join(separator) + separator :
      IsLeafDirectory ? Nodes.Join(separator) + separator :
      Nodes.Join(separator);
  }
}
