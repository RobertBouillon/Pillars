using Spin.Pillars.FileSystem.OS;
using System;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Spin.Pillars.Hierarchy;

partial class Path
{
  public static void TestParse()
  {
    AssertParsePath(@"Foo\bar", "Foo", "bar");
    AssertParsePath(@"\Foo\bar", "Foo", "bar");
    AssertParsePath(@"Foo\bar\", "Foo", "bar");
    AssertParsePath(@"\Foo\bar\", "Foo", "bar");

    AssertParsePath(@"Foo", "Foo");
    AssertParsePath(@"Foo\", "Foo");
    AssertParsePath(@"\Foo", "Foo");
    AssertParsePath(@"\Foo\", "Foo");

    AssertParsePath(@"Foo\bar\foo\bar\foo\bar", "Foo", "bar", "foo", "bar", "foo", "bar");
    AssertParsePath(String.Empty);
    AssertParsePath("\\");

    AssertParseWinPath(@"C:Foo\bar", false, false, 'C', "Foo", "bar");
    AssertParseWinPath(@"C:\Foo\bar", true, false, 'C', "Foo", "bar");
    AssertParseWinPath(@"C:Foo\bar\", false, true, 'C', "Foo", "bar");
    AssertParseWinPath(@"C:\Foo\bar\", true, true, 'C', "Foo", "bar");

    AssertParseWinPath(@"C:Foo", false, false, 'C', "Foo");
    AssertParseWinPath(@"C:Foo\", false, true, 'C', "Foo");
    AssertParseWinPath(@"C:\Foo", true, false, 'C', "Foo");
    AssertParseWinPath(@"C:\Foo\", true, true, 'C', "Foo");

    AssertParseWinPath("C:", false, false, 'C');
    AssertParseWinPath("C:\\", true, true, 'C');

    AssertParseWinPath("c:\\", true, true, 'C');

    AssertPathOrder(
      @"C:\",
      @"\",
      @"\bar\",   //List directories before files
      @"\foo\",
      @"\bar\bar\",
      @"\bar\foo\",
      @"\bar",
      @"\foo",
      @"\foo\bar",
      @"\foo\foo",
      @"C:",
      @"bar",
      @"foo"
    );
  }

  public static void TestSimplify()
  {
    AssertSimplify(@"\Foo\..", @"\");
    AssertSimplify(@"\Foo\.", @"\Foo\");
    AssertSimplify(@"\Foo\Bar\..\Foo\..\.", @"\Foo\");
    AssertSimplify(@"\Foo\Bar\..\Foo\.", @"\Foo\Foo\");
    AssertSimplify(@"\Foo\Bar\..\Foo\.", @"\Foo\Foo\");
    AssertSimplify(@"C:\Users\Robert\Code\..\Tests\Assets", @"C:\Users\Robert\Tests\Assets");
  }

  private static void AssertSimplify(string actual, string simplified)
  {
    var parsed = WindowsFilePath.Parse(actual, '\\').Simplify().ToString();
    Assert(parsed == simplified);
  }

  private static void AssertPathOrder(params string[] nodes)
  {
    var paths = nodes.Select(x => WindowsFilePath.Parse(x)).ToList();
    var ordered = paths.Order().ToList();
    Assert(paths.SequenceEqual(ordered));
  }

  private static void AssertParsePath(string path, params string[] nodes)
  {
    var parsed = Path.Parse(path, '\\');
    Assert(nodes.SequenceEqual(parsed.Nodes));
    Assert(parsed.ToString('\\') == path);
  }

  private static void AssertParseFilePath(string path, bool isRoot, bool isDir, params string[] nodes)
  {
    var filepath = WindowsFilePath.Parse(path, '\\');
    Assert(nodes.SequenceEqual(filepath.Nodes));
    Assert(isRoot == filepath.IsRooted);
    Assert(isRoot == filepath.IsTerminated);
  }

  private static void AssertParseWinPath(string path, bool isRoot, bool isDir, char drive, params string[] nodes)
  {
    var filepath = (WindowsFilePath)WindowsFilePath.Parse(path);
    var foo = filepath.ToString();

    Assert(nodes.SequenceEqual(filepath.Nodes));
    Assert(isRoot == filepath.IsRooted);
    Assert(isDir == filepath.IsTerminated);
    Assert(drive == filepath.DriveLetter);
  }
}
