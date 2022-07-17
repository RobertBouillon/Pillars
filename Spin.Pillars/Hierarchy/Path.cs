using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public partial class Path : IComparable<Path>, IFormattable
  {
    public static new Path Parse(string path, char separator)
    {
      #region Validation
      if (path is null)
        throw new ArgumentNullException(nameof(path));
      #endregion

      var isrooted = false;
      var isdir = false;
      var len = path.Length;

      var spath = path.AsSpan();

      if (path.Length == 0)
        return Empty;

      if (path == "\\")
        return Root;

      if (len > 0 && spath[0] == separator)
        isrooted = true;

      if (len > 0 && spath[len - 1] == separator)
        isdir = true;

      spath =
        isrooted && isdir ? spath.Slice(1, len - 2) :
        isrooted ? spath.Slice(1) :
        isdir ? spath.Slice(0, len - 1) :
        path;

      return new(spath.Length == 0 ? Enumerable.Empty<string>() : new String(spath).Split('\\'), isrooted, isdir);
    }

    public static IEnumerable<Path> Parse(IEnumerable<string> items)
    {
      HashSet<string> intern = new HashSet<string>();

      foreach (var item in items)
      {
        var path = new Path(item);
        path.Intern(intern);
        yield return path;
      }
    }

    public static Path Empty = new Path(Enumerable.Empty<string>());
    public static Path Root = new Path(Enumerable.Empty<string>(), true, true);

    public string[] Nodes { get; }
    public bool IsRooted { get; }
    public bool IsTerminated { get; }

    public IEnumerable<string> Branches => Nodes.Take(Nodes.Length - 1);
    public Path Branch => new(Nodes.Take(Nodes.Length - 1));
    public virtual string Leaf => Nodes.Any() ? Nodes.Last() : null;
    public int Count => Nodes.Length;

    public Path(params string[] nodes) => Nodes = nodes;
    public Path(IEnumerable<string> nodes) : this(nodes.ToArray()) { }
    public Path(ILeaf node) => Nodes = node.Traverse(x => x.Parent).Select(x => x.Name).Concat(node.Name).ToArray();
    public Path(IEnumerable<string> nodes, bool isRooted = false, bool isTerminated = false) : this(nodes) => (IsRooted, IsTerminated) = (isRooted, isTerminated);
    public Path(Path node, bool? isRooted = null, bool? isTerminated = null) : this(node.Nodes) => (IsRooted, IsTerminated) = (isRooted ?? node.IsRooted, isTerminated ?? node.IsTerminated);


    public virtual Path MoveUp(int levels = 1)
    {
      #region Validation
      if (levels < 0)
        throw new ArgumentException("Levels must be greater than 0");
      if (levels > Nodes.Length)
        throw new ArgumentException($"Cannot move up more than {Nodes.Length} levels");
      #endregion
      return new Path(Nodes.Take(Nodes.Length - levels).ToArray(), isTerminated: true);
    }

    public Path RelativeTo(Path path)
    {
      if (!Nodes.Take(path.Count).SequenceEqual(path.Nodes))
        throw new ArgumentException("path is not the root", "path");
      return new Path(Nodes.Skip(path.Count).ToArray());
    }

    public virtual Path Append(params string[] paths) => new Path(Nodes.Concat(paths).ToArray());
    public virtual Path Append(Path path) => new Path(Nodes.Concat(path.Nodes).ToArray());

    public void Intern(HashSet<string> dictionary)
    {
      for (int i = 0; i < Nodes.Length; i++)
        if (dictionary.TryGetValue(Nodes[i], out var interned))
          Nodes[i] = interned;
        else
          dictionary.Add(Nodes[i]);
    }

    public virtual int CompareTo(Path other)
    {
      int ret;

      if ((ret = IsRooted.CompareTo(other.IsRooted)) != 0)
        return -ret;

      if ((ret = IsTerminated.CompareTo(other.IsTerminated)) != 0)
        return -ret;

      if ((ret = Nodes.Length.CompareTo(other.Nodes.Length)) != 0)
        return ret;

      for (int i = 0; i < Nodes.Length; i++)
        if ((ret = Nodes[i].CompareTo(other.Nodes[i])) != 0)
          return ret;

      return 0;
    }

    public override bool Equals(object obj) => Equals((Path)obj);
    public bool Equals(Path o) =>
      Nodes.SequenceEqual(o.Nodes) &&
      IsRooted == o.IsRooted &&
      IsTerminated == o.IsTerminated;

    public override string ToString() => ToString('\\');

    public virtual string ToString(char separator) =>
      Equals(Root) ? "\\" :
      IsRooted && IsTerminated ? separator + Nodes.Join(separator) + separator :
      IsRooted ? separator + Nodes.Join(separator) :
      IsTerminated ? Nodes.Join(separator) + separator :
      Nodes.Join(separator);

    public override int GetHashCode() => Nodes.Select(x => x.GetHashCode()).Aggregate((x, y) => x ^ y);

    public string ToString(string format, IFormatProvider formatProvider) =>
      (format is null) ? ToString() :
      (format.ToLower() == "leaf") ? Leaf :
      (format.Length == 1) ? ToString(format[0]) :
      ToString();
  }
}
