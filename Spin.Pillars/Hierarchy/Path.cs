using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.Hierarchy
{
  public struct Path
  {
    public static Path Empty = new Path(Array.Empty<string>());

    public string[] Nodes { get; set; }
    public string Leaf => Nodes.Last();
    public int Count => Nodes.Length;

    public Path(IEnumerable<string> nodes) => Nodes = nodes.ToArray();
    public Path(string path, char separator) => Nodes = path.Split(separator);
    public Path(string path, string separator) => Nodes = path.Split(new[] { separator }, StringSplitOptions.None);

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
  }
}
