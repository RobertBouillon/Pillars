using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Deserializer = System.Func<System.IO.BinaryReader, System.Object>;
using Path = Spin.Pillars.Hierarchy.Path;

namespace Spin.Pillars.Logging.Readers;

public class BinaryLogReader
{
  private static Tuple<Type, Deserializer>[] DESERIALIZERS = new Tuple<Type, Deserializer>[]
  {
    Tuple.Create<Type, Deserializer>(typeof(sbyte),x=> x.ReadSByte()),
    Tuple.Create<Type, Deserializer>(typeof(short),x=> x.ReadInt16()),
    Tuple.Create<Type, Deserializer>(typeof(int),x=> x.ReadInt32()),
    Tuple.Create<Type, Deserializer>(typeof(long),x=> x.ReadInt64()),

    Tuple.Create<Type, Deserializer>(typeof(byte),x=> x.ReadByte()),
    Tuple.Create<Type, Deserializer>(typeof(ushort),x=> x.ReadUInt16()),
    Tuple.Create<Type, Deserializer>(typeof(uint),x=> x.ReadUInt32()),
    Tuple.Create<Type, Deserializer>(typeof(ulong),x=> x.ReadUInt64()),

    Tuple.Create<Type, Deserializer>(typeof(DateTime),x=>x.ReadDateTime()),
    Tuple.Create<Type, Deserializer>(typeof(TimeSpan),x=>x.ReadTimeSpan()),

    Tuple.Create<Type, Deserializer>(typeof(Path),x=>new Path(x.ReadArray<string>())),
    Tuple.Create<Type, Deserializer>(typeof(IPAddress),ParseIP),
    Tuple.Create<Type, Deserializer>(typeof(IPEndPoint),ParseEP),
  };

  private static IPAddress ParseIP(BinaryReader reader) => new IPAddress(reader.ReadArray<byte>());
  private static IPEndPoint ParseEP(BinaryReader reader)
  {
    var address = ParseIP(reader);
    var port = reader.ReadInt32();
    return new IPEndPoint(address, port);
  }

  private BinaryWriter _writer;

  private Dictionary<Type, Deserializer> Deserializers { get; } = DESERIALIZERS.ToDictionary(x => x.Item1, x => x.Item2);
  private StreamingDictionaryReader<Type> TypeReader { get; }
  private BinaryReader Reader { get; }

  public BinaryLogReader(BinaryReader reader)
  {
    #region Validation
    if (reader is null)
      throw new ArgumentNullException(nameof(reader));
    #endregion
    Reader = reader;
    TypeReader = new StreamingDictionaryReader<Type>(() => Type.GetType(Reader.ReadString()), () => Reader.ReadByte());
  }

  public LogEntry Read()
  {
    var time = Reader.ReadDateTime();
    var count = Reader.ReadInt32();
    var nodes = Enumerable.Range(0, count).Select(x => Reader.ReadString()).ToList();
    count = Reader.ReadInt32();
    var data = Enumerable.Range(0, count).Select(x => Deserializers[TypeReader.Read()](Reader));
    return new LogEntry(
      time,
      new Hierarchy.Path(nodes),
      data);
  }
}
