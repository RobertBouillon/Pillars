using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.IO.Compression;

using System.Modules.v1_0;

namespace System.Modules.Logging.Watchers
{
  public class CompressedLogFileWatcher : LogWatcher
  {
    #region Enums
    private enum RecordType
    {
      Log = 0,
      LogEntry = 1,
      FormatStringReference = 2,
      ArgumentStringReference = 3,
      LogEntryReferenced = 4
    }

    private enum DataType
    {
      Byte,
      Int16,
      Int32,
      Int64,
      UInt16,
      UInt32,
      UInt64
    }
    #endregion

    #region Private Struct Header
    private struct Header
    {
      private uint _id;
      private byte _majorVersion;
      private byte _minorVersion;
      private bool _compressed;

      public Header(int majorVersion, int minorVersion, bool compressed)
      {
        _id = 0xFFAAFFAA;
        _majorVersion = (byte)majorVersion;
        _minorVersion = (byte)minorVersion;
        _compressed = compressed;
      }

      public Header(BinaryReader reader)
      {
        _id = reader.ReadUInt32();
        _majorVersion = reader.ReadByte();
        _minorVersion = reader.ReadByte();
        _compressed = reader.ReadByte() == 1;
      }

      public void WriteTo(BinaryWriter writer)
      {
        writer.Write(_id);  //ID
        writer.Write(_majorVersion); //Major Version
        writer.Write(_minorVersion); //Minor Version
        writer.Write((byte)(_compressed ? 1 : 0));
      }
    }
    #endregion

    #region Static Methods
    public static void ReadLog(FileInfo source, Action<ILogEntry> entry)
    {
      Dictionary<String, ushort> formatStringCache = new Dictionary<string, ushort>();
      Dictionary<String, uint> argumentStringCache = new Dictionary<string, uint>();
      Dictionary<ILog, ushort> logCache = new Dictionary<ILog, ushort>();

      //
    }

    #endregion

    #region Fields
    private Dictionary<String, ushort> _formatStringCache = new Dictionary<string, ushort>();
    private Dictionary<String, uint> _argumentStringCache = new Dictionary<string, uint>();
    private Dictionary<IModule, ushort> _logCache = new Dictionary<IModule, ushort>();
    private ushort _formatStringIndex = 0;
    private uint _argumentStringIndex = 0;
    private ushort _logIndex = 0;
    private BinaryWriter _writer;
    private Stream _stream;
    private static DateTime _unixBaseTime = new DateTime(1970, 1, 1);
    private bool _compress;
    private byte[] _stringBuffer;
    private const int _stringBufferSize = ushort.MaxValue;
    #endregion

    public CompressedLogFileWatcher(IRuntime runtime, IEnumerable<IModule> logs, FileInfo file, bool compress, bool buffer)
      : this(runtime, logs, x => true, file, compress, buffer)
    {

    }

    public CompressedLogFileWatcher(IRuntime runtime, IEnumerable<IModule> modules, Func<ILogEntry, bool> filter, FileInfo file, bool compress, bool buffer)
      : base(runtime, filter, true)
    {
      if (file.Exists)
        file.Delete();

      _stringBuffer = new byte[_stringBufferSize]; 

      _stream = new FileStream(file.FullName, FileMode.CreateNew);
      if (buffer)  //Increases performance, but if the app crashes, some info may be lost. Probably redundant with filestream, but good for gZipStream. Should be tested.
        _stream = new BufferedStream(_stream);
      if (compress)
        _stream = new GZipStream(_stream, CompressionMode.Compress, true);

      _writer = new BinaryWriter(_stream);
      WriteHeader();
      //_writer = new XmlTextWriter(file.OpenWrite(), Encoding.UTF8);
      //_writer.WriteStartElement("Log");
    }

    private void WriteHeader()
    {
 	    _writer.Write(0xFFAAFFAA);  //ID
      _writer.Write((byte)0x01); //Major Version
      _writer.Write((byte)0x01); //Minor Version
      _writer.Write((byte)(_compress ? 1 : 0));
    }

    protected override void ProcessInternal(List<ILogEntry> buffer)
    {
      ushort logid, fmtid = 0;
      uint[] argids = new uint[256];
      //Not locked because it assumes the thread adding to the log watcher is also the same thread processing it.
      lock (this)
      {
        foreach (var entry in buffer)
        {
          //Store Log
          if (!_logCache.TryGetValue(entry.Module, out logid))
          {
            _writer.Write((byte)RecordType.Log);
            //_writer.Write((ushort)logid);
            WriteString(entry.Module.FullPath);
            _logCache.Add(entry.Module, fmtid = _logIndex++);
          }

          //Store format string
          if (!_formatStringCache.TryGetValue(entry.FormatText, out fmtid))
          {
            _writer.Write((byte)RecordType.FormatStringReference);
            //_writer.Write((ushort)wid);
            WriteString(entry.FormatText);
            _formatStringCache.Add(entry.FormatText, fmtid = _formatStringIndex++);
          }

          if (entry.Severity == LogSeverity.Verbose)
          {
            _writer.Write((byte)RecordType.LogEntry);
            _writer.Write((ulong)((entry.EntryTime - _unixBaseTime).TotalMilliseconds));
            _writer.Write((byte)entry.Severity);
            _writer.Write((byte)entry.Arguments.Length);
            foreach (object arg in entry.Arguments)
              WriteString(arg.ToString());
          }
          else
          {
            for (int i = 0; i < entry.Arguments.Length; i++)
            {
              string arg = entry.Arguments[i].ToString();

              if (!_argumentStringCache.TryGetValue(arg, out argids[i]))
              {
                _writer.Write((byte)RecordType.ArgumentStringReference);
                //_writer.Write((ushort)wid);
                WriteString(entry.FormatText);
                _argumentStringCache.Add(arg, argids[i] = _argumentStringIndex++);
              }
            }

            _writer.Write((byte)RecordType.LogEntry);
            _writer.Write((ulong)((entry.EntryTime - _unixBaseTime).TotalMilliseconds));
            _writer.Write((byte)entry.Severity);
            _writer.Write((byte)entry.Arguments.Length);
            for (int i = 0; i < entry.Arguments.Length; i++)
              _writer.Write(argids[i]);
          }
        }
      }
    }

    private void WriteString(string p)
    {
      int len = p.Length;
      string str = p;
      if (len > _stringBufferSize)
        str = str.Substring(0, _stringBufferSize);

      ASCIIEncoding.ASCII.GetBytes(str, 0, len, _stringBuffer, 0);
      _writer.Write((ushort)len);
      _writer.Write(_stringBuffer, 0, len);
    }

    public override void Flush()
    {
      lock (this)
      {
        _writer.Flush();
        _stream.Flush();
      }
    }

    public override void Stop()
    {
      base.Stop();
      //_writer.WriteEndElement();
      Flush();
      lock (this)
      {
        _writer.Close();
      }
    }
  }
}
