using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spin.Pillars.Workers
{
  public class BackgroundWorker : Worker
  {
    #region Fields
    private Action<WaitHandle, Func<bool>> _loop;
    private ManualResetEvent _stopHandle;
    #endregion

    #region Constructors
    public BackgroundWorker(string name, Action<WaitHandle, Func<bool>> loop) : base(name)
    {
      _loop = loop;
      _stopHandle = new ManualResetEvent(false);
    }
    #endregion

    #region Overrides

    protected override void CancelWork()
    {
    }

    protected override void Work()
    {
      _loop(_stopHandle, () => this.IsStopping);
    }

    protected override void OnStarting(CancelEventArgs e)
    {
      _stopHandle.Reset();
      base.OnStarting(e);
    }

    protected override void OnStopping(CancelEventArgs e)
    {
      _stopHandle.Set();
      base.OnStopping(e);
    }
    #endregion
  }
}
