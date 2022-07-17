using Spin.Pillars.Hierarchy;
using System;

namespace Spin.Pillars.FileSystem.OS
{
  public class WindowsFilePath : Path, IComparable<WindowsFilePath>
  {
    public static Path Parse(string path) => (path.Length > 1 && path[1] == ':') ?
      new WindowsFilePath(Path.Parse(path.Substring(2), '\\'), Char.ToUpper(path[0])) :
      Path.Parse(path, '\\');

    private char? _driveLetter;

    public char? DriveLetter
    {
      get => _driveLetter;
      set => _driveLetter = value.HasValue ? Char.ToUpper(value.Value) : null;
    }

    public WindowsFilePath(Path path, char? driveLetter = null) : base(path) => DriveLetter = driveLetter;
    public WindowsFilePath(string[] nodes, bool isRooted = false, bool isLeafDirectory = false, char? driveLetter = null) : base(nodes, isRooted, isLeafDirectory) => DriveLetter = driveLetter;
    public WindowsFilePath(Path node, bool isRooted = false, bool isLeafDirectory = false, char? driveLetter = null) : this(node.Nodes, isRooted, isLeafDirectory) => DriveLetter = driveLetter;

    public override WindowsFilePath Append(params string[] paths) => new(base.Append(paths), DriveLetter);
    public override WindowsFilePath Append(Path path) => new(base.Append(path), DriveLetter);

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

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), IsRooted, IsTerminated);

    public override bool Equals(object obj) => Equals(obj as WindowsFilePath);
    public bool Equals(WindowsFilePath o) =>
      o is not null &&
      DriveLetter == o.DriveLetter &&
      base.Equals(o);

    public override string ToString() => ToString('\\');

    public override string ToString(char separator) =>
      DriveLetter.HasValue ? $"{DriveLetter}:{base.ToString(separator)}" :
      base.ToString(separator);
  }
}
