using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.IO.Compression;
using Spin.Pillars.Logging;

namespace Spin.Pillars.Modules.Logging.Writers
{
  public class FlatLogFileWriter : TextLogWriter
  {
    private readonly FileStream _fs;
    private readonly StreamWriter _sw;

    public FlatLogFileWriter(String file) : this(new FileInfo(file)) { }
    public FlatLogFileWriter(FileInfo file)
    {
      _fs = File.Open(file.FullName, FileMode.Append, FileAccess.Write, FileShare.Read);
      _sw = new StreamWriter(_fs, Encoding.ASCII) { AutoFlush = false };
      Writer = _sw;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      _sw.Flush();
      _fs.Flush();
    }
  }
}
