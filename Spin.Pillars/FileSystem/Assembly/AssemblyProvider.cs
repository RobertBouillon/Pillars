using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using Spin.Pillars.Hierarchy;
using System.Reflection;

namespace Spin.Pillars.FileSystem.Assembly
{
  public class AssemblyProvider : Provider
  {
    private static DateStamp[] _supportedDateStamps = new DateStamp[] { DateStamp.Created, DateStamp.LastAccess, DateStamp.LastWrite };
    public override DateStamp[] SupportedDateStamps => _supportedDateStamps;
    private static string ParseDirectoryName(string fullFileName) => fullFileName.Substring(fullFileName.LastIndexOf('\\'));

    private string[] _files;            //This is how the source API returns the data
    private List<String> _directories;  //List is more efficient than [] because Array requires an additional resize.
    private HashSet<string> _fileIndex;
    private HashSet<string> _directoryIndex;
    internal string[] Files => _files ??= Assembly.GetManifestResourceNames();
    internal List<string> Directories => _directories ??= Files.Where(x => x.Contains('\\')).Select(io.Path.GetPathRoot).Distinct().ToList();
    internal HashSet<String> FileIndex => _fileIndex ??= Files.ToHashSet();
    internal HashSet<String> DirectoryIndex => _directoryIndex ??= Directories.ToHashSet();

    public System.Reflection.Assembly Assembly { get; }

    public AssemblyProvider(System.Reflection.Assembly assembly) : base(assembly.FullName) => Assembly = assembly;

    public override Directory GetDirectory(Path path) => new AssemblyDirectory(this, path);
    public override File GetFile(Path path) => new AssemblyFile(this, path);
  }
}
