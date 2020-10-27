using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules
{
  public class LogBuffer : ILogBuffer
  {
    // Can probably be replaced by a vanilla IProducerConsumerCollection<ILogBuffer>
    #region Fields
    private IProducerConsumerCollection<ILogEntry> _buffer;
    private ILogEntry _current;
    #endregion

    public LogBuffer()
    {
      _buffer = new Collections.Concurrent.ConcurrentBag<ILogEntry>();
    }

    private bool Read()
    {
      return _buffer.TryTake(out _current);
    }

    //public ILogEntry Current
    //{
    //  get { return _current; }
    //}

    public void Write(ILogEntry entry)
    {
      _buffer.Add(entry);
      //if(!_buffer.TryEnqueue(entry))
      //  throw new Exception("Log Buffer Overflow");  //To do: make this cleaner: this shouldn't except here.
    }

    public IEnumerator<ILogEntry> GetEnumerator()
    {
      while (Read())
        yield return _current;
    }

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      while (Read())
        yield return _current;
    }

    public IEnumerable<ILogEntry> Read(int max)
    {
      int count = 0;
      while (Read() && count++ < max)
        yield return _current;
    }
  }
}
