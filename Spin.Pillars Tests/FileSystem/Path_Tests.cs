using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spin.Pillars.FileSystem;
using Spin.Pillars.FileSystem.OS;
using Spin.Pillars.Hierarchy;
using System;
using System.Linq;


namespace Spin.Pillars_Tests.FileSystem
{
  [TestClass]
  public class Path_Tests
  {
    [TestMethod]
    public void TestParse()
    {
      AssertParsePath(@"Foo\bar", "Foo", "bar");
      AssertParsePath(@"\Foo\bar", "Foo", "bar");
      AssertParsePath(@"Foo\bar\", "Foo", "bar");
      AssertParsePath(@"\Foo\bar\", "Foo", "bar");

      AssertParsePath(@"Foo", "Foo");
      AssertParsePath(@"Foo\", "Foo");
      AssertParsePath(@"\Foo", "Foo");
      AssertParsePath(@"\Foo\", "Foo");

      AssertParsePath(@"Foo\bar\foo\bar\foo\bar", "Foo", "bar", "Foo", "bar", "Foo", "bar");
      AssertParsePath(String.Empty);
      AssertParsePath("\\");


      AssertParseFilePath(@"Foo\bar", false, false, "Foo", "bar");
      AssertParseFilePath(@"\Foo\bar", true, false, "Foo", "bar");
      AssertParseFilePath(@"Foo\bar\", false, true, "Foo", "bar");
      AssertParseFilePath(@"\Foo\bar\", true, true, "Foo", "bar");

      AssertParseFilePath(@"Foo", false, false, "Foo");
      AssertParseFilePath(@"Foo\", false, true, "Foo");
      AssertParseFilePath(@"\Foo", true, false, "Foo");
      AssertParseFilePath(@"\Foo\", true, true, "Foo");

      AssertParseFilePath(String.Empty, false, false);
      AssertParseFilePath("\\", true, true);


      AssertParseWinFilePath(@"C:Foo\bar", false, false, 'C', "Foo", "bar");
      AssertParseWinFilePath(@"C:\Foo\bar", true, false, 'C', "Foo", "bar");
      AssertParseWinFilePath(@"C:Foo\bar\", false, true, 'C', "Foo", "bar");
      AssertParseWinFilePath(@"C:\Foo\bar\", true, true, 'C', "Foo", "bar");

      AssertParseWinFilePath(@"C:Foo", false, false, 'C', "Foo");
      AssertParseWinFilePath(@"C:Foo\", false, true, 'C', "Foo");
      AssertParseWinFilePath(@"C:\Foo", true, false, 'C', "Foo");
      AssertParseWinFilePath(@"C:\Foo\", true, true, 'C', "Foo");

      AssertParseWinFilePath("C:", false, false, 'C');
      AssertParseWinFilePath("C:\\", true, true, 'C');

      AssertParseWinFilePath("c:\\", true, true, 'C');

      AssertPathOrder(
        @"C:\",
        @"C:",
        @"\",
        @"bar",
        @"foo",
        @"\bar\",   //List directories before files
        @"\foo\",   
        @"\bar",
        @"\foo",
        @"\bar\bar",
        @"\bar\foo",
        @"\foo\bar",
        @"\foo\foo"
      );
    }

    private void AssertPathOrder(params string[] nodes)
    {
      var paths = nodes.Select(x => FilePath.Parse(x, '\\')).ToList();
      Assert(paths.SequenceEqual(paths.Order()));
    }

    private void AssertParsePath(string path, params string[] nodes)
    {
      var parsed = Path.Parse(path, '\\');
      Assert(nodes.SequenceEqual(parsed.Nodes));
    }

    private void AssertParseFilePath(string path, bool isRoot, bool isDir, params string[] nodes)
    {
      var filepath = FilePath.Parse(path, '\\');
      Assert(nodes.SequenceEqual(filepath.Nodes));
      Assert(isRoot == filepath.IsRooted);
      Assert(isRoot == filepath.IsLeafDirectory);
    }

    private void AssertParseWinFilePath(string path, bool isRoot, bool isDir, char drive, params string[] nodes)
    {
      var filepath = (WindowsFilePath)FilePath.Parse(path, '\\');

      Assert(nodes.SequenceEqual(filepath.Nodes));
      Assert(isRoot == filepath.IsRooted);
      Assert(isRoot == filepath.IsLeafDirectory);
      Assert(drive == filepath.DriveLetter);
    }


    private void Assert(bool condition)
    {
      if (!condition)
        throw new Exception();
    }
  }
}
