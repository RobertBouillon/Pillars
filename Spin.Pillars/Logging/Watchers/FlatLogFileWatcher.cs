using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace System.Modules.Logging.Watchers
{
  public class FlatLogFileWatcher : TextWriterLogWatcher
  {
    private readonly FileStream _fs;
    private readonly StreamWriter _sw;

    public FlatLogFileWatcher(String file) : this(new FileInfo(file)) { }
    public FlatLogFileWatcher(FileInfo file) : base()
    {
      _fs = File.Open(file.FullName, FileMode.Append, FileAccess.Write, FileShare.Read);
      _sw = new StreamWriter(_fs, Encoding.ASCII) { AutoFlush = false };
      Writer = _sw;
    }

    public override void Process(IEnumerable<ILogEntry> buffer)
    {
      base.Process(buffer);
      _sw.Flush();
      _fs.Flush();
    }
  }
}
