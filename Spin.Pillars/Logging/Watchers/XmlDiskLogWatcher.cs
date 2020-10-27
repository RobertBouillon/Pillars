using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;
using System.IO;
using System.Xml;
using System.Diagnostics.UnitTesting;

namespace System.Modules.Logging.Watchers
{
  public class XmlDiskLogWatcher : LogWatcher
  {
    #region Staic Declarations
    public static void ParseFile(FileInfo file, ILogController controller)
    {
      /*
      using (XmlReader reader = new XmlTextReader(file.OpenRead()))
      {
        reader.ForEachElement(x =>{
          reader.ForEachAttribute(z =>
          {
            LogEntry entry = new LogEntry();
            switch (x)
            {
              case "time":
                entry.EntryTime = DateTime.Parse(reader.Value);
                break;
              case "severity":
                entry.Severity = EnumEx.Parse<LogSeverity>(reader.Value);
                break;
              case "category":
                entry.Module = controller.GetLog(reader.Value);
                break;
              case "format":
                entry.FormatText = reader.Value;
                break;
              default:
                throw new Exception(String.Format("Unknown field: {0}", x));
            }
            reader.Read();
            List<string> args = new List<string>();
            reader.ForEachElement(y =>
            {
              args.Add(reader.Value);
            });
            entry.Arguments = args.Cast<object>().ToArray();
          });
        });
      }*/
      throw new NotImplementedException();
    }
    #endregion
    #region Fields
    private XmlTextWriter _writer;
    private FileStream _stream;
    #endregion
    #region Constructors
    public XmlDiskLogWatcher(IRuntime runtime, Func<ILogEntry, bool> filter, FileInfo file)
      : base(runtime, filter, true)
    {
      if (file.Exists)
        file.Delete();

      //_writer = new XmlTextWriter(file.OpenWrite(), Encoding.UTF8);
      _stream = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      _writer = new XmlTextWriter(_stream, Encoding.UTF8);
      _writer.WriteStartElement("Log");
    }
    #endregion

    #region Methods

    public override void Reprocess(Action<ILogEntry> callback)
    {
      //Must happen on the same thread as Process!

      var eos = _stream.Position;
      _stream.Position = 0;
      XmlTextReader reader = new XmlTextReader(_stream);
      reader.Read();//Log Element
      while (_stream.Position != eos)
        callback(ReadNextLogEntry(reader));
    }

    private ILogEntry ReadNextLogEntry(XmlTextReader reader)
    {
      /*
      ILogEntry ret = new LogEntry();
      LogController temp = new LogController();

      reader.Read();
      reader.ForEachAttribute(x => {
        switch (reader.Name)
        {
          case "time":
            ret.EntryTime = DateTime.Parse(reader.Value);
            break;
          case "severity":
            ret.Severity = EnumEx.Parse<LogSeverity>(reader.Value);
            break;
          case "module":
            ret.Module = temp.GetLog(reader.Value);
            break;
          case "format":
            ret.FormatText = reader.Value;
            break;
          default:
            throw new FormattedException("Unknown attribute: {0}", reader.Name);
        }
      });

      reader.ForEachElement(x => {
        switch(reader.Name)
        {
          case "Arguments":
            List<string> args = new List<string>();
            reader.ForEachElement(y => {
              args.Add(reader.Value);
            });
            ret.Arguments = args.ToArray();
            break;
          case "Error":
            ExceptionInfo ex = new ExceptionInfo();
            ret.Error = ex;
            ImportException(reader, ex);
            break;
          default:
            throw new FormattedException("Unknown element: {0}", reader.Name);
        }
      });
      return ret;
       * */
      throw new NotImplementedException();
    }

    protected override void ProcessInternal(List<ILogEntry> buffer)
    {
      //Not locked because it assumes the thread adding to the log watcher is also the same thread processing it.
      lock (this)
      {
        foreach (var entry in buffer.OrderByDescending(x => x.EntryTime))
        {
          _writer.WriteStartElement("LogEntry");

          _writer.WriteAttributeString("time", entry.EntryTime.ToString("r"));
          _writer.WriteAttributeString("severity", entry.Severity.ToString());
          _writer.WriteAttributeString("log", entry.Module.FullPath);

          _writer.WriteElementString("Format", entry.FormatText);

          _writer.WriteStartElement("Arguments");
          foreach (string arg in entry.Arguments)
            _writer.WriteElementString("Argument", arg);
          _writer.WriteEndElement();

          if (entry.Error != null)
            ExportException(entry.Error);

          _writer.WriteEndElement();
        }
      }
    }

    private void ExportException(IExceptionInfo ex)
    {
      _writer.WriteStartElement("Error");
      _writer.WriteAttributeString("message", ex.Message);
      _writer.WriteElementString("StackTrace", ex.StackTrace);
      _writer.WriteElementString("VerboseInfo", ex.VerboseInfo);

      if (ex.Data != null)
      {
        if (ex.Data.Count > 0)
        {
          _writer.WriteStartElement("Data");
          foreach (var data in ex.Data)
          {
            _writer.WriteStartElement("Item");
            _writer.WriteAttributeString("name", data.ToString());
            _writer.WriteCData(ex.Data[data].ToString());
            _writer.WriteEndElement();
          }
          _writer.WriteEndElement();
        }
      }

      if (ex.InnerException != null)
        ExportException(ex.InnerException);
      _writer.WriteEndElement();
    }

    private void ImportException(XmlTextReader reader, ExceptionInfo ex)
    {
      reader.ForEachAttribute(x => {
        switch (reader.Name)
        {
          case "message":
            ex.Message = reader.Value;
            break;
          default:
            throw new Exception(String.Format("Unknown attribute: {0}", reader.Name));
        }
      });

      reader.ForEachElement(x => {
        switch (reader.Name)
        {
          case "StackTrace":
            ex.StackTrace = reader.Value;
            break;
          case "VerboseInfo":
            ex.VerboseInfo = reader.Value;
            break;
          case "Data":
            reader.Read();
            Dictionary<string, string> data = new Dictionary<string, string>();
            reader.ForEachElement(y => {
              var name = reader.GetAttribute("name");
              reader.Read();
              var value = reader.Value;
              data[name] = value;
            });
            break;
          case "InnerException":
            var exception = new ExceptionInfo();
            ex.InnerException = exception;
            ImportException(reader, exception);
            break;
          default:
            break;
        }
      });
      reader.Read();
    }

    public override void Flush()
    {
      _writer.Flush();
    }

    public override void Stop()
    {
      base.Stop();
      _writer.WriteEndElement();
      _writer.Flush();
      _writer.Close();
    }
    #endregion

    #region Unit Test Methods
    //public static void TestInstance(LogController controller)
    //{
    //  FileInfo tmpfile = null;
    //  try
    //  {
    //    tmpfile = new FileInfo(Path.GetTempFileName());
    //    XmlDiskLogWatcher watcher = new XmlDiskLogWatcher(controller.AllLogs, x => true, tmpfile);
    //    controller.AllWatchers.Add(watcher);
    //    watcher.Start();

    //    var log = controller.CreateLog("XmlTester");
    //    log.Write("Test");
    //    log.Write("Test {0}", "2");

    //    try
    //    {
    //      FormattedException ex = new FormattedException("Test exception");
    //      throw ex;
    //    }
    //    catch(FormattedException ex)
    //    {
    //      log.Write(ex, "Text Exception {0}", 3);
    //    }


    //    int num = 1;
    //    watcher.Reprocess(x =>
    //      {
    //        switch (num)
    //        {
    //          case 1:
    //            if (x.GetFormattedString() != "test")
    //              throw new UnitTestException("Test case 1 failed");
    //            break;
    //          case 2:
    //            if (x.GetFormattedString() != "Test 2")
    //              throw new UnitTestException("Test case 2 failed");
    //            break;
    //          case 3:
    //            if (x.GetFormattedString() != "Test Exception")
    //              throw new UnitTestException("Test case 3 failed");
    //            if(x.Error == null)
    //              throw new UnitTestException("Test case 3 failed");
    //            if(x.Error.Message!="Test Exception")
    //              throw new UnitTestException("Test case 3 failed");
    //            break;
    //          default:
    //            throw new UnitTestException("Not implemented");
    //        }
    //        num++;
    //      });

    //  }
    //  finally
    //  {
    //    tmpfile.Delete();
    //  }
    //}
    #endregion
  }
}
