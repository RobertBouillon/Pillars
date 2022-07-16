using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.OS
{
  public class WindowsFilePath : FilePath, IComparable<WindowsFilePath>
  {
    public static new WindowsFilePath Parse(string path, string separator) => new(path.Split(new[] { separator }, StringSplitOptions.None));
    private char? _driveLetter;

    public char? DriveLetter
    {
      get => _driveLetter; 
      set => _driveLetter = value.HasValue ? Char.ToUpper(value.Value) : null;
    }

    public WindowsFilePath(FilePath path, char driveLetter) : this(path) => DriveLetter = driveLetter;
    public WindowsFilePath(FilePath path)
    {
      IsLeafDirectory = path.IsLeafDirectory;
      IsRooted = path.IsRooted;
      Nodes = path.Nodes;
    }
    public WindowsFilePath(Path path) => Nodes = path.Nodes;
    public WindowsFilePath(params string[] nodes) : base(nodes) { }
    public WindowsFilePath(string[] nodes, bool isRooted = false, bool isLeafDirectory = false, char? driveLetter = null) : base(nodes, isRooted, isLeafDirectory) => DriveLetter = driveLetter;
    public WindowsFilePath(IEnumerable<string> nodes) : base(nodes) { }
    public WindowsFilePath(ILeaf node) : base(node) { }
    public WindowsFilePath(Path node, bool isRooted = false, bool isLeafDirectory = false, char? driveLetter = null) : this(node.Nodes, isRooted, isLeafDirectory) => DriveLetter = driveLetter;

    public override WindowsFilePath Append(params string[] paths) => new(base.Append(paths)) { DriveLetter = DriveLetter };
    public override WindowsFilePath Append(Path path) => new(base.Append(path)) { DriveLetter = DriveLetter };

    public override WindowsFilePath MoveUp(int levels = 1) => new WindowsFilePath(base.MoveUp(), IsRooted, true, DriveLetter);

    public int CompareTo(WindowsFilePath other)
    {
      int ret;

      if (DriveLetter is null && other.DriveLetter is not null)
        return 1;

      if (DriveLetter is not null && other.DriveLetter is null)
        return -1;

      if ((ret = DriveLetter.Value.CompareTo(other.DriveLetter)) != 0)
        return ret;

      return base.CompareTo(other);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), IsRooted, IsLeafDirectory);

    public override bool Equals(object obj) => Equals(obj as WindowsFilePath);
    public bool Equals(WindowsFilePath o) =>
      o is not null &&
      DriveLetter == o.DriveLetter &&
      base.Equals(o);

    public override string ToString() => ToString('\\');

    public override string ToString(char separator) =>
      DriveLetter.HasValue ? $"{DriveLetter}:{base.ToString()}" :
      base.ToString();
  }
}
