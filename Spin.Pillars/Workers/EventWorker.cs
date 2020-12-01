using System.Threading;

namespace Spin.Pillars.Workers
{
  public abstract class EventWorker : Worker
  {
    protected abstract WaitHandle Handle { get; }
    public EventWorker() { }
    public EventWorker(string name) : base(name) { }
    protected override void WaitForWork() => Handle.WaitOne(WaitDelay);
  }
}
