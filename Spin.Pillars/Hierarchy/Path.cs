using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public struct Path : IComparable<Path>, IFormattable
  {
    public static Path Parse(string path, char separator) => new(path.Split(separator));
    public static Path Parse(string path, string separator) => new(path.Split(new[] { separator }, StringSplitOptions.None));

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

    public static Path Empty = new Path(Array.Empty<string>());

    public string[] Nodes { get; set; }
    public IEnumerable<string> Branches => Nodes.Take(Nodes.Length - 1);
    public Path Branch => new(Nodes.Take(Nodes.Length - 1));
    public string Leaf => Nodes.Any() ? Nodes.Last() : null;
    public int Count => Nodes.Length;

    public Path(params string[] nodes) => Nodes = nodes;
    public Path(IEnumerable<string> nodes) => Nodes = nodes.ToArray();
    public Path(ILeaf node) => Nodes = node.Traverse(x => x.Parent).Select(x => x.Name).Concat(node.Name).ToArray();

    public Path MoveUp(int levels = 1)
    {
      #region Validation
      if (levels < 0)
        throw new ArgumentException("Levels must be greater than 0");
      if (levels > Nodes.Length)
        throw new ArgumentException($"Cannot move up more than {Nodes.Length} levels");
      #endregion
      return new Path(Nodes.Take(Nodes.Length - levels).ToArray());
    }

    public Path RelativeTo(Path path)
    {
      if (!Nodes.Take(path.Count).SequenceEqual(path.Nodes))
        throw new ArgumentException("path is not the root", "path");
      return new Path(Nodes.Skip(path.Count).ToArray());
    }

    public Path Append(params string[] paths) => new Path(Nodes.Concat(paths).ToArray());
    public Path Append(Path path) => new Path(Nodes.Concat(path.Nodes).ToArray());

    public string ToString(char separator) => String.Join(separator.ToString(), Nodes);
    public string ToString(string separator) => String.Join(separator, Nodes);

    public void Intern(HashSet<string> dictionary)
    {
      for (int i = 0; i < Nodes.Length; i++)
        if (dictionary.TryGetValue(Nodes[i], out var interned))
          Nodes[i] = interned;
        else
          dictionary.Add(Nodes[i]);
    }

    public int CompareTo(Path other)
    {
      int ret;

      if ((ret = Nodes.Length.CompareTo(other.Nodes.Length)) != 0)
        return ret;

      for (int i = 0; i < Nodes.Length; i++)
        if ((ret = Nodes[i].CompareTo(other.Nodes[i])) != 0)
          return ret;

      return 0;
    }

    public override bool Equals(object obj) => Equals((Path)obj);
    public bool Equals(Path o) => Nodes.SequenceEqual(o.Nodes);
    public override int GetHashCode() => Nodes.Select(x => x.GetHashCode()).Aggregate((x, y) => x ^ y);
    public override string ToString() => Nodes.Join('\\');

    public string ToString(string format, IFormatProvider formatProvider) =>
      (format.ToLower() == "leaf") ? Leaf :
      (format.Length == 1) ? ToString(format[0]) :
      ToString();
  }
}
