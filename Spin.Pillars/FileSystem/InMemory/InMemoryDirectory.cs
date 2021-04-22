using System;
using System.Collections.Generic;
using System.Linq;
using io = System.IO;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.InMemory
{
  public class InMemoryDirectory : Directory
  {
    //Caching
    public override bool Cache() => (_memoryDirectory = FileSystem.FindDirectory(Path)) is not null;
    public override void ClearCache() => _memoryDirectory = null;
    public override bool IsCached => _memoryDirectory is not null;
    private MemoryDirectory _memoryDirectory;
    internal MemoryDirectory MemoryDirectory => _memoryDirectory ?? FileSystem.FindDirectory(Path);

    public override InMemoryFileSystem FileSystem => base.FileSystem as InMemoryFileSystem;
    public override InMemoryDirectory Parent => Path.Count == 0 ? null : new InMemoryDirectory(FileSystem, Path.MoveUp());
    public override void Purge()
    {
      MemoryDirectory.Files.Clear();
      MemoryDirectory.Directories.Clear();
    }

    internal InMemoryDirectory(InMemoryFileSystem fileSystem, MemoryDirectory directory) : base(fileSystem, directory.Path) => _memoryDirectory = directory;
    public InMemoryDirectory(InMemoryFileSystem fileSystem, Path path) : base(fileSystem, path) { }
  }
}