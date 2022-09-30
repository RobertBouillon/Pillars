//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace Spin.Pillars.Workers
//{
//  public abstract class QueueWorker<T> : Worker, IEnumerable<T>
//  {
//    private ConcurrentQueue<T> _queue;
//    private AutoResetEvent _workHandle = new AutoResetEvent(false);
//    private T _item;
//    protected bool HasWork => _queue.Count > 0;

//    public QueueWorker(string name) : base(name)
//    {
//      #region Validation
//      if (queue == null)
//        throw new ArgumentNullException(nameof(queue));
//      #endregion
//      _queue = new ConcurrentQueue<T>();
//    }

//    public virtual void Enqueue(T item)
//    {
//      _queue.Enqueue(item);
//      _workHandle.Set();
//    }

//    public abstract void Work(T item);

//    public void Flush()
//    {
//      while (_queue.Count > 0)
//        Thread.Sleep(WaitDelay);
//    }

//    protected override void Work()
//    {
//      if (_queue.TryDequeue(out T item))
//        Work(_item = item);
//    }

//    #region Events

//    #region WorkPerformedEventArgs Subclass
//    public class WorkPerformedEventArgs : EventArgs
//    {
//      #region Fields
//      private readonly TimeSpan _duration;
//      private readonly T _item;
//      #endregion
//      #region Properties
//      public T Item
//      {
//        get { return _item; }
//      }

//      public TimeSpan Duration
//      {
//        get { return _duration; }
//      }
//      #endregion
//      #region Constructors
//      internal WorkPerformedEventArgs(TimeSpan duration, T item)
//      {
//        #region Validation
//        if (duration == null)
//          throw new ArgumentNullException("duration");
//        if (item == null)
//          throw new ArgumentNullException("item");
//        #endregion
//        _duration = duration;
//        _item = item;
//      }
//      #endregion
//    }
//    #endregion

//    public new event EventHandler<WorkPerformedEventArgs> Worked;
//    protected override void OnWorked(Worker.WorkPerformedEventArgs e)
//    {
//      Worked?.Invoke(this, new WorkPerformedEventArgs(e.Duration, _item));
//      base.OnWorked(e);
//    }

//    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
//    IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
//    #endregion
//  }
//}
