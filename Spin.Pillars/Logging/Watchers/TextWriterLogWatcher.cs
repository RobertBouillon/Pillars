using System;
using System.Collections.Generic;
using System.IO;
using System.Modules.v1_0;
using System.Linq;

namespace System.Modules.Logging.Watchers
{
  public class TextWriterLogWatcher : LogWatcher
  {
    #region Fields
    private TextWriter _writer;
    #endregion
    #region Properties
    public string Delimiter { get; set; } = "\t";
    public Func<ILogEntry, string> Formatter { get; set; }
    public Func<ILogEntry, bool> Filter { get; set; }
    protected TextWriter Writer
    {
      get => _writer;
      set => _writer = value;
    }
    #endregion

    #region Constructors
    protected TextWriterLogWatcher()
    {
      Formatter = x => String.Format(@"{0:hh\:mm\:ss\:ffffff}{3}{1}{3}{2}", x.EntryTime, x.Module.FullPath, x.ToString(), Delimiter);
    }

    public TextWriterLogWatcher(TextWriter writer) : this()
    {
      #region Validation
      if (writer == null)
        throw new ArgumentNullException(nameof(writer));
      #endregion

      _writer = writer;
    }
    #endregion

    #region Overrides
    public override void Process(IEnumerable<ILogEntry> buffer)
    {
      foreach (var entry in buffer.Where(x => Filter == null || Filter(x)).OrderBy(x => x.EntryTime))
      {
        _writer.WriteLine(Formatter(entry));
        if (entry.Error != null)
          _writer.WriteLine(entry.Error.VerboseInfo);
      }
    }
    #endregion

    #region Methods
    public override void Dispose()
    {
      lock (this)
      {
        if (_writer != null)
        {
          _writer.Flush();
          _writer.Close();
        }
      }
    }
    #endregion
  }
}
