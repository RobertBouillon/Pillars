using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  public abstract class Observer : Disposable
  {
    public ChangeTypes Changes { get; }

    public Observer(ChangeTypes changes) => Changes = changes;

    #region ChangedEventArgs Subclass
    public class ChangedEventArgs : EventArgs
    {
      public IEntity Entity { get; private set; }
      public ChangeTypes Change { get; private set; }
      internal ChangedEventArgs(IEntity entity, ChangeTypes type) => (Entity, Change) = (entity, type);
    }
    #endregion

    public event global::System.EventHandler<ChangedEventArgs> Changed;
    protected void OnChanged(IEntity entity, ChangeTypes type) => OnChanged(new ChangedEventArgs(entity, type));
    protected virtual void OnChanged(ChangedEventArgs e) => Changed?.Invoke(this, e);
  }
}
