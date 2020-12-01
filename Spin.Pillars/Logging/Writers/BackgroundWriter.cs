using Spin.Pillars.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static System.FluentTry;

namespace Spin.Pillars.Logging.Writers
{
  public class BackgroundWriter : LogWriter, IDisposable
  {
    #region Worker
    class Worker : TimedWorker
    {
      private readonly ConcurrentQueue<LogEntry> _queue;
      private readonly List<LogEntry> _buffer;
      private readonly LogWriter _writer;

      internal Worker(ConcurrentQueue<LogEntry> queue, LogWriter writer, int bufferSize) : base("Background Log Writer", TimeSpan.FromMilliseconds(100))
      {
        _queue = queue;
        _writer = writer;
        _buffer = new List<LogEntry>(bufferSize);
      }

      protected override void Work() => Flush();

      private void Flush()
      {
        _buffer.Clear();
        var count = _queue.Count;

        if (count == 0)
          return;

        for (int i = 0; i < count; i++)
          if (_queue.TryDequeue(out var log)) //Will only return false on concurrent dequeues. Since we're multi-producer, single-consumer, this is fine.
            _buffer.Add(log);

        Try(() => _writer.Write(_buffer.OrderBy(x => x.EntryTime))).Catch(OnError);
      }

      protected override void OnError(ErrorEventArgs e)
      {
        //We can't write this to the log because that could result in an infinite loop :)
        Console.WriteLine($"Error in log writer: {e.Exception}");
        base.OnError(e);
      }

      protected override void OnStopped(EventArgs e)
      {
        Flush();
        base.OnStopped(e);
      }
    }
    #endregion

    private ConcurrentQueue<LogEntry> _queue = new ConcurrentQueue<LogEntry>();
    private Worker _worker;
    private Disposer _disposer = new Disposer();

    public BackgroundWriter(LogWriter writer, int bufferSize = 4096)
    {
      #region Validation
      if (writer is null)
        throw new ArgumentNullException(nameof(writer));
      #endregion

      _worker = new Worker(_queue, writer, bufferSize);
      _worker.Start();
    }

    public override void Write(IEnumerable<LogEntry> entries)
    {
      foreach (var entry in entries)
        _queue.Enqueue(entry);
    }

    public override void Write(LogEntry entry) => _queue.Enqueue(entry);

    public void Dispose()
    {
      if (_disposer.TryDispose())
        _worker.Stop();
    }
  }
}
