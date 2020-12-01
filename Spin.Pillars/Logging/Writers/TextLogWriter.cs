using Spin.Pillars.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spin.Pillars.Logging.Writers
{
  public class TextLogWriter : LogWriter, IDisposable
  {
    private Disposer _disposer = new Disposer();

    public string Delimiter { get; set; }
    public Func<LogEntry, string> Formatter { get; set; }
    protected TextWriter Writer { get; set; }

    protected TextLogWriter() { }
    public TextLogWriter(TextWriter writer, Func<LogEntry, string> formatter = null, string delimiter = "\t")
    {
      #region Validation
      if (writer == null)
        throw new ArgumentNullException(nameof(writer));
      #endregion

      Writer = writer;
      Delimiter = delimiter;
      Formatter = formatter ?? (x => String.Format(@"{0:hh\:mm\:ss\:ffffff}{3}{1}{3}{2}", x.EntryTime, x.Module.Path.ToString('\\'), x.ToString(), Delimiter));
    }

    public override void Write(IEnumerable<LogEntry> buffer)
    {
      foreach (var entry in buffer)
      {
        Writer.WriteLine(Formatter(entry));
        if (entry.Error != null)
          Writer.WriteLine(entry.Error.VerboseInfo);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      Writer.Flush();
      Writer.Close();
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      if (_disposer.TryDispose())
        Dispose(true);
    }
  }
}
