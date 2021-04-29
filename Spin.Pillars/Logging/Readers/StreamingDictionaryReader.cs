using System;
using System.Collections.Generic;

namespace Spin.Pillars.Logging.Readers
{
  public class StreamingDictionaryReader<T>
  {
    private List<T> _items;
    public Func<T> _valueReader;
    public Func<int> _indexReader;

    public StreamingDictionaryReader(Func<T> valueReader, Func<int> indexReader)
    {
      _valueReader = valueReader;
      _indexReader = indexReader;
      _items = new List<T>();
      _items.Add(default);

    }

    public T Read()
    {
      var index = _indexReader();
      if (index > 0)
        _items.Add(_valueReader());

      return _items[index];
    }
  }
}
