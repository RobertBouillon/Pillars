using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using io = System.IO;

namespace Spin.Pillars.FileSystem.InMemory
{
  public class InMemoryFileSystem : FileSystem
  {
    private static TimeStamp[] TIME_STAMPS = new[] { TimeStamp.LastAccess, TimeStamp.LastWrite, TimeStamp.Created };

    public override IEnumerable<TimeStamp> SupportedDateStamps => TIME_STAMPS;
    private MemoryDirectory _root;

    private char _pathSeparator = io.Path.DirectorySeparatorChar;
    public override char PathSeparator => _pathSeparator;

    public InMemoryFileSystem() => _root = new MemoryDirectory(PathSeparator.ToString());
    public InMemoryFileSystem(char pathSeparator) : this() => _pathSeparator = pathSeparator;

    public override Directory GetDirectory(FilePath path) => new InMemoryDirectory(this, path);
    public override File GetFile(FilePath path) => new InMemoryFile(this, path);

    internal MemoryFile FindFile(FilePath path)
    {
      var dir = _root;
      foreach (var item in path.Branches)
        if (!dir.Directories.TryGetValue(item, out dir))
          return null;

      if (!dir.Files.TryGetValue(path.Leaf, out var file))
        return null;

      return file;
    }

    internal bool TryFindFile(FilePath path, out MemoryFile file) => (file = FindFile(path)) is not null;

    internal MemoryDirectory FindDirectory(FilePath path)
    {
      var dir = _root;
      foreach (var item in path.Nodes)
        if (dir.Directories.TryGetValue(item, out var existing))
          dir = existing;
        else
          throw new Exception($"'{item}' in '{dir.Path}' not found");

      return dir;
    }

    internal bool TryFindDirectory(FilePath path, out MemoryDirectory dir)
    {
      dir = _root;
      foreach (var item in path.Nodes)
        if (!dir.Directories.TryGetValue(item, out dir))
          return false;

      return true;
    }

    public override bool FileExists(FilePath path) => TryFindFile(path, out var _);
    public override bool DirectoryExists(FilePath path) => TryFindDirectory(path, out var _);
    public override void DeleteFile(FilePath path)
    {
      var file = FindFile(path);
      if (file is null)
        throw new Exception("File not found");
      file.Directory.Files.Remove(file.Name);
    }

    public override void DeleteDirectory(FilePath path)
    {
      var dir = FindDirectory(path);

      if (dir.Parent is null)
        throw new Exception("Cannot delete the root directory");

      dir.Parent.Directories.Remove(dir.Name);
    }

    public override void CreateFile(FilePath path)
    {
      var name = path.Leaf;
      var parent = FindParentDirectory(path);
      if (parent.Directories.ContainsKey(name))
        throw new Exception($"A directory '{name}' already exists in '{parent}'");
      parent.CreateFile(name);
    }

    public override void CreateDirectory(FilePath path)
    {
      if (!path.Nodes.Any())
        throw new Exception("Cannot create root directory");
      if (DirectoryExists(path))
        throw new Exception($"A directory '{path.Leaf}' already exists in '{path.MoveUp().Leaf}'");

      var sub = _root;
      foreach (var item in path.Nodes)
        if (sub.Directories.TryGetValue(item, out var existing))
          sub = existing;
        else
          sub.Directories.Add(item, sub = new(sub, item));
    }

    private MemoryDirectory FindParentDirectory(FilePath path)
    {
      var dir = _root;
      if (path.Nodes.Length > 1)
        dir = FindDirectory(path.MoveUp());

      if (dir is null)
        throw new Exception($"Path '{path.Branch}' does not exist");

      return dir;
    }

    public override IEnumerable<FilePath> GetFiles(FilePath directory) => FindDirectory(directory).Files.Select(x => x.Value.Path);
    public override IEnumerable<FilePath> GetDirectories(FilePath directory) => FindDirectory(directory).Directories.Select(x => x.Value.Path);
  }
}
