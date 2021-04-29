using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serializer = System.Action<System.Object, System.IO.BinaryWriter>;
using Path = Spin.Pillars.Hierarchy.Path;

namespace Spin.Pillars.Logging.Writers
{
  public class BinaryLogWriter
  {
    private static Tuple<Type, Serializer>[] SERIALIZERS = new Tuple<Type, Serializer>[]
    {
      Tuple.Create<Type, Serializer>(typeof(sbyte),(x,y)=>y.Write((int)x)),
      Tuple.Create<Type, Serializer>(typeof(short),(x,y)=>y.Write((short)x)),
      Tuple.Create<Type, Serializer>(typeof(int),(x,y)=>y.Write((int)x)),
      Tuple.Create<Type, Serializer>(typeof(long),(x,y)=>y.Write((long)x)),

      Tuple.Create<Type, Serializer>(typeof(byte),(x,y)=>y.Write((byte)x)),
      Tuple.Create<Type, Serializer>(typeof(ushort),(x,y)=>y.Write((ushort)x)),
      Tuple.Create<Type, Serializer>(typeof(uint),(x,y)=>y.Write((uint)x)),
      Tuple.Create<Type, Serializer>(typeof(ulong),(x,y)=>y.Write((ulong)x)),

      Tuple.Create<Type, Serializer>(typeof(DateTime),(x,y)=>y.Write((DateTime)x)),
      Tuple.Create<Type, Serializer>(typeof(TimeSpan),(x,y)=>y.Write((TimeSpan)x)),

      Tuple.Create<Type, Serializer>(typeof(Path),(x,y)=>y.Write(((Path)x).Nodes)),
    };

    private BinaryWriter _writer;

    private Dictionary<Type, Serializer> _serializers = SERIALIZERS.ToDictionary(x => x.Item1, x => x.Item2);
    private StreamingDictionaryWriter<Type> _typeDictionary;

    public BinaryLogWriter(BinaryWriter writer)
    {
      #region Validation
      if (writer is null)
        throw new ArgumentNullException(nameof(writer));
      #endregion

      _writer = writer;
      _typeDictionary = new StreamingDictionaryWriter<Type>(x => _writer.Write(x.FullName), x => _writer.Write((byte)x));
    }

    public void Write(LogEntry entry)
    {
      _writer.Write(entry.Time);
      _writer.Write((byte)entry.Scope.Count);
      foreach (var item in entry.Scope.Nodes)
        _writer.Write(item);

      _writer.Write((byte)entry.Data.Count);
      foreach (var item in entry.Scope.Nodes)
      {
        var type = item.GetType();
        if (!_serializers.TryGetValue(type, out var serializer))
          throw new Exception($"No serializer for '{item.GetType()}'");
        _typeDictionary.Write(type);
        serializer(item, _writer);
      }
    }
  }
}
