using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Writers;

public class StreamingDictionaryWriter<T>
{
  public Action<T> _valueWriter;
  public Action<int> _indexWriter;
  private Dictionary<T, int> _dictionary;
  int _index;

  public StreamingDictionaryWriter(Action<T> valueWriter, Action<int> indexWriter)
  {
    _valueWriter = valueWriter;
    _indexWriter = indexWriter;
    _dictionary = new Dictionary<T, int>();
  }

  public void Write(T value)
  {
    if (_dictionary.TryGetValue(value, out var index))
    {
      _indexWriter(index);
    }
    else
    {
      _dictionary.Add(value, ++_index);
      _indexWriter(0);
      _valueWriter(value);
    }
  }
}
