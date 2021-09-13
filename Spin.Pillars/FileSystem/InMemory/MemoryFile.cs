using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spin.Pillars.Hierarchy;
using io = System.IO;
using System.IO;

namespace Spin.Pillars.FileSystem.InMemory
{
  internal class MemoryFile : IDisposable
  {
    internal class SafeStream : Stream
    {
      public MemoryStream Stream { get; }

      public SafeStream(MemoryStream stream) => Stream = stream;

      public override bool CanRead => Stream.CanRead;
      public override bool CanSeek => Stream.CanSeek;
      public override bool CanWrite => Stream.CanWrite;
      public override long Length => Stream.Length;

      public override long Position
      {
        get => Stream.Position;
        set => Stream.Position = value;
      }

      public override void Flush() => Stream.Flush();
      public override int Read(byte[] buffer, int offset, int count) => Stream.Read(buffer, offset, count);
      public override long Seek(long offset, SeekOrigin origin) => Stream.Seek(offset, origin);
      public override void SetLength(long value) => Stream.SetLength(value);
      public override void Write(byte[] buffer, int offset, int count) => Stream.Write(buffer, offset, count);
      protected override void Dispose(bool disposing) => Position = 0;
      public void RealDispose() => Stream.Dispose();
    }

    public SafeStream Stream { get; }
    private string _name;
    public MemoryDirectory Directory { get; set; }

    public string Name
    {
      get => _name;
      set
      {
        if (_name == value)
          return;
        Directory.Files.Remove(_name);
        _name = value;
        Directory.Files.Add(_name, this);
      }
    }

    public FileSize Size => new(Stream.Stream.Length);
    public bool IsReadOnly { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime CreationTime { get; set; }
    public Hierarchy.Path Path => new(Directory.Traverse(x => x.Parent).Where(x => x.Parent is not null).Reverse().Select(x => x.Name).Concat(Name).ToArray());

    public MemoryFile(MemoryDirectory directory, string name)
    {
      Directory = directory;
      _name = name;
      Stream = new SafeStream(new MemoryStream());
    }

    private MemoryFile(string name, io.MemoryStream stream, MemoryDirectory parent)
    {
      _name = name;
      Stream = new(stream);
      Directory = parent;
    }

    public void Delete() => Directory.Files.Remove(_name);

    public MemoryFile Clone(MemoryDirectory dir = null) => new MemoryFile(_name, Stream.Stream.Clone(), dir ?? Directory);

    public void Dispose() => Stream.Dispose();
  }
}
